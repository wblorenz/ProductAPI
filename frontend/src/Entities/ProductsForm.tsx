import { useState } from 'react'
import type { Product } from './ProductsTable'

interface ProductsFormProps {
    values: Product | null;
    errorMessage?: string | null;
    onSave: (updatedProduct: Product) => void;
    onCancel: () => void;
}

function ProductsForm({ values, errorMessage, onSave, onCancel }: ProductsFormProps) {
    const [name, setName] = useState(values?.name || '');
    const [description, setDescription] = useState(values?.description || '');
    const [price, setPrice] = useState(values?.price || 0);

    const getSaveValues = (): Product => {
        const prod = values ? { ...values } : { id: 0, name: '', description: '', price: 0 };
        return { ...prod, name, description, price: Number.isNaN(price) ? 0 : price };
    };

    return (
        <div>
            {errorMessage && (
                <div className="error-message" style={{ whiteSpace: 'pre-line' }}>
                    {errorMessage}
                </div>
            )}

            <div>
                <label htmlFor="name">Name:</label>
                <input id="name" type="text" value={name} onChange={(e) => setName(e.target.value)} />
            </div>

            <div>
                <label htmlFor="description">Description:</label>
                <input id="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>

            <div>
                <label htmlFor="price">Price:</label>
                <input id="price" type="number" value={price} onChange={(e) => setPrice(parseFloat(e.target.value))} />
            </div>

            <div style={{ marginTop: '1rem' }}>
                <button onClick={() => onSave(getSaveValues())}>Save</button> <button onClick={() => onCancel()}>Cancel</button>
            </div>
        </div>
    );
}

export default ProductsForm;