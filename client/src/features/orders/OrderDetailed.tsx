import { Box, Button, Grid, Typography } from "@mui/material";
import { Order } from "../../app/models/order"
import BasketTable from "../basket/BasketTable";
import { BasketItem } from "../../app/models/basket";
import BasketSummary from "../basket/BasketSummary";

interface Props{
    order:Order;
    setSelectedOrder:(id:number) => void;
}
export default function OrderDetailed({order,setSelectedOrder}:Props){
    const subtotal = order.orderItems.reduce((sum,item) => sum + (item.quantity * item.price),0) ?? 0;
    return (
<>
        <Box display='flex' justifyContent='space-between'>
            <Typography sx={{p:2}} gutterBottom variant="h4" > #Order {order.id} - {order.orderStatus}</Typography>
            <Button onClick={() => setSelectedOrder(0)} sx={{m:2}} size="large" variant="contained">Back to orders</Button>
        </Box>
        <BasketTable items={order.orderItems as BasketItem[]} IsBasket={false}/>
        <Grid container>
            <Grid item xs={6} />
            <Grid item xs={6}>
                <BasketSummary subtotal={subtotal} />
            </Grid>
        </Grid>
</>
    )
}