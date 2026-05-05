import { useRef, useEffect } from 'react';
import type { CartSummary } from '../../types';
import './CartPanel.css';

interface CartPanelProps {
  cart: CartSummary;
  loading: boolean;
  error: string | null;
  onClose: () => void;
  updateQuantity: (productId: number, quantity: number) => Promise<void>;
}

export function CartPanel({ cart, loading, error, onClose, updateQuantity }: CartPanelProps) {
  const closeButtonRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    closeButtonRef.current?.focus();
  }, []);

  function handleQuantityChange(productId: number, value: string) {
    const qty = parseInt(value, 10);
    if (!isNaN(qty) && qty >= 1 && qty <= 5) {
      updateQuantity(productId, qty);
    }
  }

  return (
    <div role="dialog" aria-label="Shopping cart" aria-modal="true" className="cart-panel">
      <div className="cart-panel__header">
        <h2 className="cart-panel__title">Your Cart</h2>
        <button
          ref={closeButtonRef}
          className="cart-panel__close"
          aria-label="Close cart"
          onClick={onClose}
        >
          ✕
        </button>
      </div>

      {loading && <p className="cart-panel__loading">Loading cart…</p>}

      {error && (
        <p role="alert" className="cart-panel__error">
          {error}
        </p>
      )}

      {!loading && cart.items.length === 0 && (
        <p className="cart-panel__empty">Your cart is empty.</p>
      )}

      {!loading && cart.items.length > 0 && (
        <>
          <ul className="cart-panel__items">
            {cart.items.map((item) => (
              <li key={item.productId} className="cart-panel__item">
                <span className="cart-panel__item-name">{item.productName}</span>
                <div className="cart-panel__item-row">
                  <input
                    type="number"
                    min={1}
                    max={5}
                    value={item.quantity}
                    aria-label={`Quantity for ${item.productName}`}
                    className="cart-panel__quantity-input"
                    onChange={(e) => handleQuantityChange(item.productId, e.target.value)}
                  />
                  <span className="cart-panel__unit-price">${item.unitPrice.toFixed(2)}</span>
                  <span className="cart-panel__item-total">${item.totalPrice.toFixed(2)}</span>
                </div>
              </li>
            ))}
          </ul>
          <div className="cart-panel__subtotal">
            <strong>Total: ${cart.subtotal.toFixed(2)}</strong>
          </div>
        </>
      )}
    </div>
  );
}
