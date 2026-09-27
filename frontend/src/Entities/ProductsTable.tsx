import { useEffect, useState } from 'react'

import ProductsForm from './ProductsForm';


export interface Product {
    id: number;
    price: number;
    description: string;
    name: string;
}
function ProductsTable() {
    const [products, setProducts] = useState<Product[]>([]);

    const [isEditing, setIsEditing] = useState(false);

    const [editingProduct, setEditingProduct] = useState<Product | null>(null);
    


    const editRecord = (product: Product) => {
        setIsEditing(true);
        setEditingProduct(product);
    }



    const onSaveProduct = (updatedProduct: Product) => {
        if (updatedProduct.id > 0) {
            fetch('/api/products/' + updatedProduct.id, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedProduct),
            }).then(() => {
                setIsEditing(false);
                setEditingProduct(null);
                updateData();
            });
        } else {
            fetch('/api/products', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(updatedProduct),
            }).then(() => {
                setIsEditing(false);
                setEditingProduct(null);
                updateData();
            });
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

        if(confirm('Are you sure you want to delete the record?')){        
            fetch('/api/products/' + id, { method: 'DELETE' }).then(() => { updateData() });
        }
    }

    return (
    <div>

        {!isEditing &&
        <table>
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
                        <td><button onClick={() => editRecord(product)}>Edit</button> <button onClick={() => deleteRecord(product.id)}> Delete</button></td>
                    </tr>
                ))}
            </tbody>
        </table>}

        {isEditing && <ProductsForm values={editingProduct} onSave={onSaveProduct} onCancel={() => {setIsEditing(false); setEditingProduct(null)}} />}
    </div>
    );
}

export default ProductsTable;