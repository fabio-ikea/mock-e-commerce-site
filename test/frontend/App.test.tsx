import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { App } from '../../src/frontend/src/App';
import type { Product, CartSummary } from '../../src/frontend/src/types';

const mockProducts: Product[] = [
  {
    id: 1,
    name: 'Test Headphones',
    description: 'Great sound quality.',
    price: 79.99,
    category: 'Electronics',
    stock: 10,
    imageUrl: 'https://example.com/headphones.jpg',
  },
];

const emptyCartSummary: CartSummary = { items: [], itemCount: 0, subtotal: 0 };

vi.mock('../../src/frontend/src/hooks/useProducts');
vi.mock('../../src/frontend/src/hooks/useCart');
vi.mock('../../src/frontend/src/api');

import { useProducts } from '../../src/frontend/src/hooks/useProducts';
import { useCart } from '../../src/frontend/src/hooks/useCart';
import { addToCart } from '../../src/frontend/src/api';

const mockedUseProducts = vi.mocked(useProducts);
const mockedUseCart = vi.mocked(useCart);
const mockedAddToCart = vi.mocked(addToCart);

const defaultCartHook = {
  cart: emptyCartSummary,
  loading: false,
  error: null,
  refresh: vi.fn().mockResolvedValue(undefined),
  updateQuantity: vi.fn().mockResolvedValue(undefined),
};

describe('App', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('renders the header with shop name', () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByText('Mock Shop')).toBeInTheDocument();
  });

  it('renders the hero banner', () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByText(/discover quality products/i)).toBeInTheDocument();
  });

  it('renders the products section heading', () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByRole('heading', { name: /our products/i })).toBeInTheDocument();
  });

  it('shows loading state', () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: true, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByText(/loading products/i)).toBeInTheDocument();
  });

  it('shows error state', () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: 'Network error' });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByText(/error: network error/i)).toBeInTheDocument();
  });

  it('renders product list when loaded', () => {
    mockedUseProducts.mockReturnValue({ products: mockProducts, loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);

    expect(screen.getByText('Test Headphones')).toBeInTheDocument();
  });

  it('shows notification after adding to cart', async () => {
    mockedUseProducts.mockReturnValue({ products: mockProducts, loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);
    mockedAddToCart.mockResolvedValue(emptyCartSummary);

    render(<App />);
    await userEvent.click(screen.getByRole('button', { name: /add test headphones to cart/i }));

    expect(await screen.findByRole('status')).toHaveTextContent('"Test Headphones" added to cart!');
  });

  it('shows error notification when add to cart fails', async () => {
    mockedUseProducts.mockReturnValue({ products: mockProducts, loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);
    mockedAddToCart.mockRejectedValue(new Error('Cannot exceed 5 units per product.'));

    render(<App />);
    await userEvent.click(screen.getByRole('button', { name: /add test headphones to cart/i }));

    expect(await screen.findByRole('status')).toHaveTextContent('Cannot exceed 5 units per product.');
  });

  // ─── US1 & Polish: CartPanel integration ────────────────────────────────

  it('renders CartPanel when cart icon is clicked', async () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);
    await userEvent.click(screen.getByRole('button', { name: /shopping cart/i }));

    expect(screen.getByRole('dialog', { name: /shopping cart/i })).toBeInTheDocument();
  });

  it('removes CartPanel when onClose is triggered', async () => {
    mockedUseProducts.mockReturnValue({ products: [], loading: false, error: null });
    mockedUseCart.mockReturnValue(defaultCartHook);

    render(<App />);
    await userEvent.click(screen.getByRole('button', { name: /shopping cart/i }));
    expect(screen.getByRole('dialog')).toBeInTheDocument();

    await userEvent.click(screen.getByRole('button', { name: /close cart/i }));

    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });
});

