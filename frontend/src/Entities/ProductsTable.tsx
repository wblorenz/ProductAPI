import { useEffect, useState } from 'react'

import ProductsForm from './ProductsForm';

export interface Product {
    id: number;
    price: number;
    description: string;
    name: string;
}

interface ValidationErrorItem {
    MemberNames?: string[];
    ErrorMessage?: string;
}

async function extractErrorMessage(response: Response): Promise<string> {
    try {
        const text = await response.text();
        if (!text) {
            return `Error: ${response.status} ${response.statusText}`;
        }

        let data: unknown;
        try {
            data = JSON.parse(text);
        } catch {
            return text;
        }

        // Handle stringified JSON (e.g. JsonSerializer.Serialize written as JSON)
        if (typeof data === 'string') {
            const rawString = data;
            try {
                data = JSON.parse(rawString);
            } catch {
                return rawString;
            }
        }

        if (Array.isArray(data)) {
            const messages = data
                .map((item: ValidationErrorItem | string) => {
                    if (typeof item === 'string') return item;
                    return item.ErrorMessage || '';
                })
                .filter(Boolean);

            if (messages.length > 0) {
                return messages.join('\n');
            }
        } else if (typeof data === 'object' && data !== null) {
            const obj = data as Record<string, unknown>;
            if (typeof obj.ErrorMessage === 'string') return obj.ErrorMessage;
            if (typeof obj.title === 'string') return obj.title;
            if (typeof obj.detail === 'string') return obj.detail;
        }

        return typeof data === 'string' ? data : JSON.stringify(data);
    } catch {
        return `Error: ${response.status} ${response.statusText}`;
    }
}

function ProductsTable() {
    const [products, setProducts] = useState<Product[]>([]);
    const [isEditing, setIsEditing] = useState(false);
    const [editingProduct, setEditingProduct] = useState<Product | null>(null);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const editRecord = (product: Product) => {
        setErrorMessage(null);
        setEditingProduct(product);
        setIsEditing(true);
    }

    const newRecord = () => {
        setErrorMessage(null);
        setEditingProduct(null);
        setIsEditing(true);
    }

    const onSaveProduct = async (updatedProduct: Product) => {
        setErrorMessage(null);
        try {
            const url = updatedProduct.id > 0 ? `/api/products/${updatedProduct.id}` : '/api/products';
            const method = updatedProduct.id > 0 ? 'PUT' : 'POST';

            const response = await fetch(url, {
                method,
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedProduct),
            });

            if (!response.ok) {
                const message = await extractErrorMessage(response);
                setErrorMessage(message);
                return; // Do not return the user to the listview on error
            }

            setErrorMessage(null);
            setIsEditing(false);
            setEditingProduct(null);
            updateData();
        } catch (error) {
            setErrorMessage(error instanceof Error ? error.message : 'An unexpected error occurred while saving.');
        }
    }

    const updateData = (): void => {
        fetch('/api/products')
            .then(response => response.json())
            .then(data => setProducts(data));
    }

    useEffect(() => {
        updateData();
    }, []);

    const deleteRecord = (id: number) => {
        if (confirm('Are you sure you want to delete the record?')) {        
            fetch('/api/products/' + id, { method: 'DELETE' }).then(() => { updateData() });
        }
    }

    return (
        <div>
            {!isEditing && (
                <>
                    <div style={{ marginBottom: '1rem' }}>
                        <button onClick={newRecord}>New Product</button>
                    </div>
                    <table style={{ border: '1px solid white' }}>
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Description</th>
                                <th>Cost</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {products.map(product => (
                                <tr key={product.id}>
                                    <td>{product.name}</td>
                                    <td>{product.description}</td>
                                    <td>${product.price}</td>
                                    <td>
                                        <button onClick={() => editRecord(product)}>Edit</button>{' '}
                                        <button onClick={() => deleteRecord(product.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}

            {isEditing && (
                <ProductsForm
                    key={editingProduct?.id ?? 'new'}
                    values={editingProduct}
                    errorMessage={errorMessage}
                    onSave={onSaveProduct}
                    onCancel={() => {
                        setErrorMessage(null);
                        setIsEditing(false);
                        setEditingProduct(null);
                    }}
                />
            )}
        </div>
    );
}

export default ProductsTable;