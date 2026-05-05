import { useState, useEffect, useCallback } from 'react';
import { fetchCart, updateCartQuantity } from '../api';
import type { CartSummary } from '../types';

const defaultCart: CartSummary = { items: [], itemCount: 0, subtotal: 0 };

export function useCart() {
  const [cart, setCart] = useState<CartSummary>(defaultCart);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setLoading(true);
    try {
      const data = await fetchCart();
      setCart(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load cart.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  async function updateQuantity(productId: number, quantity: number): Promise<void> {
    try {
      const updated = await updateCartQuantity(productId, { quantity });
      setCart(updated);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update cart.');
    }
  }

  return { cart, loading, error, refresh, updateQuantity };
}
