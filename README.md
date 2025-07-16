# Blazor WebAssembly PayPal Integration Demo

This project demonstrates how to integrate PayPal payment processing into a Blazor WebAssembly application using a secure API backend architecture.


<a href="https://buymeacoffee.com/alex_m" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174"></a>

## Project Structure

The solution consists of two main projects:

- **BlazorWasm-PayPal-Demo-App**: Client-side Blazor WebAssembly application
- **BlazorWasm-PayPal-Demo-Api**: .NET API backend that handles secure PayPal API calls

## Features

- Complete PayPal checkout flow integration
- Secure handling of PayPal credentials (Client Secret never exposed to client)
- Interactive payment buttons with JavaScript interop
- Comprehensive payment lifecycle management
- Responsive UI built with Bootstrap

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK or later
- A PayPal Developer account

### Configuration

1. **API Configuration**:
   Edit `appsettings.json` in BlazorWasm-PayPal-Demo-Api:
   ```json
   "PayPal": {
     "ClientId": "<YOUR-PAYPAL-CLIENT-ID>",
     "ClientSecret": "<YOUR-PAYPAL-CLIENT-SECRET>"
   }
   ```

2. **Client Configuration**:
   Edit `wwwroot/appsettings.json` in BlazorWasm-PayPal-Demo-App:
   ```json
   "PayPal": {
     "ClientId": "<YOUR-PAYPAL-CLIENT-ID>"
   }
   ```

3. **Run Both Applications**:
   Use AppDev StartConfiguration to launch both applications.

## PayPal Checkout Process

1. **Create Order**: When the user clicks the PayPal button, the `createOrder` function is called to create a payment order through the API.
2. **PayPal Login**: The user is redirected to PayPal's login page to authenticate and approve the payment.
3. **Approve Payment**: After authenticating, the user approves the payment details, triggering the `onApprove` callback.
4. **Capture Order**: The API backend captures the approved payment, finalizing the transaction.

## Technical Implementation

### Client Side
- `wwwroot/javascript/paypal-integration.js` handles PayPal SDK integration
- Uses JavaScript interop to render PayPal buttons and handle user interactions

### UI Component
- `Pages/PayPalDemo.razor` implements callback methods:
  - `OnPaymentApproved` - Handles successful payments
  - `OnPaymentCancelled` - Handles cancelled payments
  - `OnPaymentError` - Handles payment errors

### Backend API
- Securely communicates with PayPal APIs using client credentials
- Handles order creation and capture operations
- Protects sensitive credentials from client-side exposure

## PayPal Developer Account

To obtain your Client ID and Client Secret, you need to create a PayPal Developer account at [developer.paypal.com](https://developer.paypal.com) and set up an application.

For testing purposes, it's recommended to use PayPal's Sandbox environment, which allows you to simulate transactions without using real money.

## Security Considerations

- The Client Secret is never exposed to the browser
- All sensitive API calls that require authentication are handled by the backend API service
- HTTPS is used for all communications between the client, API, and PayPal

## License

[MIT License](LICENSE)

## Acknowledgements

- [PayPal Developer Documentation](https://developer.paypal.com/docs/checkout/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

# Support the Project

If you find this project helpful, consider supporting its development! Your support helps maintain and improve this learning demo.

## Buy Me a Coffee

Enjoy using the demo? You can show your appreciation by buying me a coffee:

[<i class="bi bi-cup-hot-fill"></i> Buy Me a Coffee](https://coff.ee/Alex_M)

## PayPal

Alternatively, you can contribute via PayPal:

[<i class="bi bi-paypal"></i> PayPal.me](https://paypal.me/MayerAlexAndDer)

Every contribution, no matter how small, makes a difference and is greatly appreciated! Thx!