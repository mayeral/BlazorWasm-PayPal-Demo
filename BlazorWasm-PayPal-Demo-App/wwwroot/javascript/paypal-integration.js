window.paypalFunctions = {
    loadPayPalScript: function (dotNetCallback, clientId, amount, currency, description, baseUrl) {
        {

            // Remove any existing PayPal script
            const existingScript = document.getElementById('paypal-script');
            if (existingScript) {
                existingScript.remove();
            }

            // Clear the container
            const container = document.getElementById('paypal-button-container');
            if (container) {
                container.innerHTML = '';
            }

            // Create and load the PayPal script
            const script = document.createElement('script');
            script.id = 'paypal-script';
            script.src = "https://www.paypal.com/sdk/js?client-id=" + clientId + "&currency=" + currency;

            // Set up the PayPal button
            script.onload = function () {
                paypal.Buttons({

                    //  styling options here
                    style: {
                        shape: 'pill',
                        color: 'blue',
                        layout: 'vertical'
                    },


                    // create the order
                    createOrder: async function () {
                        try {
                            // Call the .NET method directly instead of using fetch
                            const orderData = await dotNetCallback.invokeMethodAsync('CreateOrderAsync', {
                                amount: amount,
                                currency: currency,
                                description: description,
                                returnUrl: baseUrl + 'paypal-demo?success=true',
                                cancelUrl: baseUrl + 'paypal-demo?cancel=true'
                            });

                            // Return JUST the ID string, not the object
                            if (orderData && orderData.id) {
                                return orderData.id;  // Return just the string
                            } else {
                                throw new Error('No order ID received from server');
                            }

                            // Return the order ID from the response
                            return orderData.id;
                        }
                        catch (error) {
                            dotNetCallback.invokeMethodAsync('OnPaymentError', error.toString());
                            return null;
                        }
                    },

                    // callback for when the payment is approved
                    onApprove: function (data) {
                        return dotNetCallback.invokeMethodAsync('OnPaymentApproved', data.orderID);
                    },

                    // callback for when the payment is cancelled
                    onCancel: function () {
                        return dotNetCallback.invokeMethodAsync('OnPaymentCancelled');
                    },

                    // callback for when the payment fails
                    onError: function (err) {
                        return dotNetCallback.invokeMethodAsync('OnPaymentError', err.toString());
                    }
                }).render('#paypal-button-container');
            };

            document.body.appendChild(script);
        }
    }
};