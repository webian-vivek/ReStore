import { useState, useEffect } from "react";
import { Product } from "../../app/models/product";
import ProductList from "./ProductList";

export default function Catalog() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    fetch("http://localhost:5000/api/products")
      .then((Response) => Response.json())
      .then((data) => setProducts(data));
  }, []); // not use [] then every time call this
  return (
    <>
      <ProductList products={products} />
    </>
  );
}
