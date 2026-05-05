import type { Product, AddToCartRequest, CartSummary, UpdateCartQuantityRequest } from '../types';

const BASE_URL = '/api';

async function handleCartErrorResponse(response: Response): Promise<never> {
  const contentType = response.headers.get('content-type') ?? '';
  if (contentType.includes('application/problem+json')) {
    const problem = await response.json() as Record<string, unknown>;
    const detail = (problem['detail'] ?? problem['title'] ?? 'Cart error') as string;
    throw new Error(detail);
  }
  const text = await response.text();
  throw new Error(text || `Request failed with status ${response.status}`);
}

export async function fetchProducts(): Promise<Product[]> {
  const response = await fetch(`${BASE_URL}/products`);
  if (!response.ok) throw new Error('Failed to fetch products');
  return response.json();
}

export async function fetchProductById(id: number): Promise<Product> {
  const response = await fetch(`${BASE_URL}/products/${id}`);
  if (!response.ok) throw new Error(`Failed to fetch product ${id}`);
  return response.json();
}

export async function fetchCart(): Promise<CartSummary> {
  const response = await fetch(`${BASE_URL}/cart`);
  if (!response.ok) throw new Error('Failed to fetch cart');
  return response.json();
}

export async function addToCart(request: AddToCartRequest): Promise<CartSummary> {
  const response = await fetch(`${BASE_URL}/cart`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) return handleCartErrorResponse(response);
  return response.json();
}

export async function updateCartQuantity(
  productId: number,
  request: UpdateCartQuantityRequest,
): Promise<CartSummary> {
  const response = await fetch(`${BASE_URL}/cart/${productId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) return handleCartErrorResponse(response);
  return response.json();
}

