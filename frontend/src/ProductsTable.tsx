import { useEffect, useState } from 'react'


interface Product {
    id: number;
    price: number;
    description: string;
    name: string;
}
function ProductsTable() {
    const [products, setProducts] = useState<Product[]>([]);
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
            fetch('api/products/' + id, { method: 'DELETE' }).then(() => { updateData() });
        }
    }

    return (
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
                        <td><button>Edit</button> <button onClick={() => deleteRecord(product.id)}> Delete</button></td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default ProductsTable;