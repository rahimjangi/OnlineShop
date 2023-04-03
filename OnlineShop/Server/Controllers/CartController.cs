﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Shared;

namespace OnlineShop.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("products")]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = await _cartService.GetCartProducts(cartItems);
        return Ok(result);
    }

    [HttpPost, Authorize]
    public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
    {
        var result = await _cartService.StoreCartItems(cartItems);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
    {
        return Ok(await _cartService.GetCartItemsCount());
    }

}
