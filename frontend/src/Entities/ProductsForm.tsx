import { useState } from 'react'
import type { Product } from './ProductsTable'

interface ProductsFormProps {
    values: Product | null;
    onSave: (updatedProduct: Product) => void;
    onCancel: () => void;
}

function ProductsForm({ values, onSave, onCancel }: ProductsFormProps) {

    const [name, setName] = useState(values?.name || '');
    const [description, setDescription] = useState(values?.description || '');
    const [price, setPrice] = useState(values?.price || 0);
    const getSaveValues = (): Product => {
        const prod = values ? { ...values } : { id: 0, name: '', description: '', price: 0 };
        return { ...prod, name, description, price: Number.isNaN(price) ? 0 : price };
    };
    return (
        <div>
        <label htmlFor="name">Name:</label>
        <input type="text" value={name} onChange={(e) => setName(e.target.value)} />

        <label htmlFor="description">Description:</label>
        <input type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
        <label htmlFor="price">Price:</label>
        <input type="number" value={price} onChange={(e) => setPrice(parseFloat(e.target.value))} />

        <button onClick={() => onSave(getSaveValues())}>Save</button>
        <button onClick={() => onCancel()}>Cancel</button>
        </div>
    );
}

export default ProductsForm;