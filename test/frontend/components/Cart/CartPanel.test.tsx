import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { CartPanel } from '../../../../src/frontend/src/components/Cart';
import type { CartSummary } from '../../../../src/frontend/src/types';

const emptyCart: CartSummary = { items: [], itemCount: 0, subtotal: 0 };

const populatedCart: CartSummary = {
  items: [
    { productId: 1, productName: 'Wireless Headphones', unitPrice: 79.99, quantity: 2, totalPrice: 159.98 },
    { productId: 2, productName: 'Running Shoes', unitPrice: 59.99, quantity: 3, totalPrice: 179.97 },
  ],
  itemCount: 5,
  subtotal: 339.95,
};

describe('CartPanel', () => {
  // ─── US1: Display cart contents ─────────────────────────────────────────

  it('renders each item productName', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText('Wireless Headphones')).toBeInTheDocument();
    expect(screen.getByText('Running Shoes')).toBeInTheDocument();
  });

  it('renders each item unitPrice', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText('$79.99')).toBeInTheDocument();
    expect(screen.getByText('$59.99')).toBeInTheDocument();
  });

  it('renders each item totalPrice', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText('$159.98')).toBeInTheDocument();
    expect(screen.getByText('$179.97')).toBeInTheDocument();
  });

  it('renders the cart subtotal', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText(/\$339\.95/)).toBeInTheDocument();
  });

  it('renders empty-cart message when items array is empty', () => {
    render(<CartPanel cart={emptyCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText(/your cart is empty/i)).toBeInTheDocument();
  });

  it('does not render items list when cart is empty', () => {
    render(<CartPanel cart={emptyCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.queryByRole('list')).not.toBeInTheDocument();
  });

  it('calls onClose when close button is clicked', async () => {
    const onClose = vi.fn();
    render(<CartPanel cart={emptyCart} loading={false} error={null} onClose={onClose} updateQuantity={vi.fn()} />);

    await userEvent.click(screen.getByRole('button', { name: /close cart/i }));

    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it('shows loading state', () => {
    render(<CartPanel cart={emptyCart} loading={true} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByText(/loading/i)).toBeInTheDocument();
  });

  // ─── US2: Quantity controls ──────────────────────────────────────────────

  it('renders a quantity input for each cart item', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    const inputs = screen.getAllByRole('spinbutton');
    expect(inputs).toHaveLength(2);
  });

  it('quantity input has accessible aria-label', () => {
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByRole('spinbutton', { name: /quantity for wireless headphones/i })).toBeInTheDocument();
  });

  it('calls updateQuantity with correct productId and value on change', () => {
    const updateQuantity = vi.fn().mockResolvedValue(undefined);
    render(<CartPanel cart={populatedCart} loading={false} error={null} onClose={() => {}} updateQuantity={updateQuantity} />);

    const input = screen.getByRole('spinbutton', { name: /quantity for wireless headphones/i });
    fireEvent.change(input, { target: { value: '3' } });

    expect(updateQuantity).toHaveBeenCalledWith(1, 3);
  });

  // ─── US3: Error display ──────────────────────────────────────────────────

  it('renders error message when error prop is set', () => {
    render(<CartPanel cart={emptyCart} loading={false} error="Cannot exceed 5 units per product." onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.getByRole('alert')).toHaveTextContent('Cannot exceed 5 units per product.');
  });

  it('does not render error region when error is null', () => {
    render(<CartPanel cart={emptyCart} loading={false} error={null} onClose={() => {}} updateQuantity={vi.fn()} />);

    expect(screen.queryByRole('alert')).not.toBeInTheDocument();
  });
});
