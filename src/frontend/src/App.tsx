import { useState, useRef, useEffect } from 'react';
import type { Product } from './types';
import { Header } from './components/Header';
import { HeroBanner } from './components/HeroBanner';
import { ProductList } from './components/ProductList';
import { CartPanel } from './components/Cart';
import { useProducts } from './hooks/useProducts';
import { useCart } from './hooks/useCart';
import { addToCart } from './api';
import './App.css';

export function App() {
  const { products, loading, error } = useProducts();
  const { cart, loading: cartLoading, error: cartError, refresh: refreshCart, updateQuantity } = useCart();
  const [cartOpen, setCartOpen] = useState(false);
  const [cartMessage, setCartMessage] = useState<string | null>(null);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, []);

  async function handleAddToCart(product: Product) {
    try {
      await addToCart({ productId: product.id, quantity: 1 });
      await refreshCart();
      setCartMessage(`"${product.name}" added to cart!`);
      if (timerRef.current) clearTimeout(timerRef.current);
      timerRef.current = setTimeout(() => setCartMessage(null), 3000);
    } catch (err) {
      setCartMessage(err instanceof Error ? err.message : 'Failed to add item to cart.');
    }
  }

  return (
    <div className="app">
      <Header
        cartItemCount={cart.itemCount}
        onCartOpen={() => setCartOpen(true)}
      />

      {cartOpen && (
        <CartPanel
          cart={cart}
          loading={cartLoading}
          error={cartError}
          onClose={() => setCartOpen(false)}
          updateQuantity={updateQuantity}
        />
      )}

      <HeroBanner />

      <main className="app__main">
        <h1 className="app__section-heading">Our products</h1>

        {cartMessage && (
          <div className="app__notification" role="status">
            {cartMessage}
          </div>
        )}

        {loading && <p className="app__loading">Loading products…</p>}
        {error && <p className="app__error">Error: {error}</p>}
        {!loading && !error && (
          <ProductList products={products} onAddToCart={handleAddToCart} />
        )}
      </main>
    </div>
  );
}

