import { renderHook, act, waitFor } from '@testing-library/react';
import { useCart } from '../../../src/frontend/src/hooks/useCart';
import type { CartSummary } from '../../../src/frontend/src/types';

vi.mock('../../../src/frontend/src/api', () => ({
  fetchCart: vi.fn(),
  updateCartQuantity: vi.fn(),
}));

import { fetchCart, updateCartQuantity } from '../../../src/frontend/src/api';
const mockedFetchCart = vi.mocked(fetchCart);
const mockedUpdateCartQuantity = vi.mocked(updateCartQuantity);

const emptyCart: CartSummary = { items: [], itemCount: 0, subtotal: 0 };
const populatedCart: CartSummary = {
  items: [{ productId: 1, productName: 'Headphones', unitPrice: 79.99, quantity: 2, totalPrice: 159.98 }],
  itemCount: 2,
  subtotal: 159.98,
};

describe('useCart', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  // ─── US1: Initial load ───────────────────────────────────────────────────

  it('starts with loading true and empty cart', () => {
    mockedFetchCart.mockReturnValue(new Promise(() => {}));

    const { result } = renderHook(() => useCart());

    expect(result.current.loading).toBe(true);
    expect(result.current.cart.items).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it('returns cart data after successful fetch', async () => {
    mockedFetchCart.mockResolvedValue(populatedCart);

    const { result } = renderHook(() => useCart());

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.cart.items).toEqual(populatedCart.items);
    expect(result.current.cart.itemCount).toBe(2);
    expect(result.current.error).toBeNull();
  });

  it('calls fetchCart on mount', async () => {
    mockedFetchCart.mockResolvedValue(emptyCart);

    renderHook(() => useCart());

    await waitFor(() => expect(mockedFetchCart).toHaveBeenCalledTimes(1));
  });

  it('exposes a refresh function that reloads the cart', async () => {
    mockedFetchCart.mockResolvedValue(emptyCart);
    const { result } = renderHook(() => useCart());
    await waitFor(() => expect(result.current.loading).toBe(false));

    mockedFetchCart.mockResolvedValue(populatedCart);
    await act(async () => { await result.current.refresh(); });

    expect(result.current.cart.itemCount).toBe(2);
  });

  // ─── US2: updateQuantity ─────────────────────────────────────────────────

  it('calls updateCartQuantity with correct args and refreshes cart', async () => {
    mockedFetchCart.mockResolvedValue(emptyCart);
    const updatedCart: CartSummary = { ...populatedCart };
    mockedUpdateCartQuantity.mockResolvedValue(updatedCart);

    const { result } = renderHook(() => useCart());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => { await result.current.updateQuantity(1, 3); });

    expect(mockedUpdateCartQuantity).toHaveBeenCalledWith(1, { quantity: 3 });
    expect(result.current.cart.itemCount).toBe(updatedCart.itemCount);
  });

  it('clears error on successful updateQuantity', async () => {
    mockedFetchCart.mockResolvedValue(emptyCart);
    mockedUpdateCartQuantity.mockResolvedValue(populatedCart);

    const { result } = renderHook(() => useCart());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => { await result.current.updateQuantity(1, 3); });

    expect(result.current.error).toBeNull();
  });

  // ─── US3: Error states ───────────────────────────────────────────────────

  it('sets error when fetchCart fails', async () => {
    mockedFetchCart.mockRejectedValue(new Error('Failed to fetch cart'));

    const { result } = renderHook(() => useCart());

    await waitFor(() => expect(result.current.loading).toBe(false));
    expect(result.current.error).toBeTruthy();
  });

  it('sets error when updateCartQuantity rejects', async () => {
    mockedFetchCart.mockResolvedValue(emptyCart);
    mockedUpdateCartQuantity.mockRejectedValue(new Error('Cannot exceed 5 units per product.'));

    const { result } = renderHook(() => useCart());
    await waitFor(() => expect(result.current.loading).toBe(false));

    await act(async () => { await result.current.updateQuantity(1, 6); });

    expect(result.current.error).toBe('Cannot exceed 5 units per product.');
  });

  it('clears error on next successful fetch via refresh', async () => {
    mockedFetchCart
      .mockRejectedValueOnce(new Error('network error'))
      .mockResolvedValue(emptyCart);

    const { result } = renderHook(() => useCart());
    await waitFor(() => expect(result.current.error).toBeTruthy());

    await act(async () => { await result.current.refresh(); });

    expect(result.current.error).toBeNull();
  });
});
