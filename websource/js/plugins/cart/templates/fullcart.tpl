﻿[% if(CartProducts.length === 0) { %]
<div class="fullcart-empty">
    [%= localize('shoppingCartNoProducts')%]</div>
[% } %] 

[% if(CartProducts.length > 0) { %]
<table class="fullcart">
    <thead>
        <tr>
            <th class="fullcart-name" colspan="2">
                [%= localize('shoppingCartName')%]
            </th>
            <th class="fullcart-price">
                [%= localize('shoppingCartPricePerUnit')%]
            </th>
            <th class="fullcart-count">
                [%= localize('shoppingCartCount')%]
            </th>
            <th class="fullcart-cost">
                [%= localize('shoppingCartCost')%]
            </th>
            <th class="fullcart-delete">
                <a href="javascript:void(0);" data-cart-clear="true" class="cross" title="[%= localize('shoppingCartClear')%]">
                </a>
            </th>
        </tr>
    </thead>
    <tbody>
        [% for(var i=0, arrLength = CartProducts.length; i< arrLength; i++) { %]
        <tr data-itemid="[%= CartProducts[i].ShoppingCartItemId%]">
            <td class="fullcart-photo-data">
                <a href="[%= CartProducts[i].Link%]">[%= CartProducts[i].Photo%]</a>
            </td>
            <td class="fullcart-name-data">
                <div>
                    <a href="[%= CartProducts[i].Link%]" class="link-pv-name">[%= CartProducts[i].Name%]</a></div>
                <div class="sku">
                    [%= CartProducts[i].SKU %]</div>
                [% if(CartProducts[i].ColorName != null) {%]
                <div> [%= ColorHeader + ': '  + CartProducts[i].ColorName%]</div>
                [% } %] 
                
                [% if(CartProducts[i].SizeName != null) {%]
                <div> [%= SizeHeader + ': ' + CartProducts[i].SizeName%]</div>
                [% } %] 
                
                [%= CartProducts[i].SelectedOptions%]
            </td>
            <td class="fullcart-price-data">
                <span class="price">[%= CartProducts[i].Price %]</span>
            </td>
            <td class="fullcart-count-data">
                [% if (CartProducts[i].OrderByRequest === true) { %] [%= CartProducts[i].Amount%]
                [% }else{ %] <span class="input-wrap">
                    <input data-cart-itemcount="true" data-plugin="spinbox" type="text" value="[%= CartProducts[i].Amount%]" data-spinbox-options="{min:[%= CartProducts[i].MinAmount%],max:[%=CartProducts[i].MaxAmount%],step:[%=CartProducts[i].Multiplicity%]}"></span>
                [% } %]
                <div class="not-available cart-padding">
                    [%= CartProducts[i].Avalible%]</div>
            </td>
            <td class="fullcart-cost-data">
                <span class="price">[%= CartProducts[i].Cost%]</span>
            </td>
            <td class="fullcart-delete-data">
                <a href="javascript:void(0);" data-cart-remove="[%= CartProducts[i].ShoppingCartItemId%]" class="cross"
                    title="[%= localize('shoppingCartDeleteProduct') %]"></a>
            </td>
        </tr>
        [%}%] 
        <tr>
            <td colspan="3" class="fullcart-cupon">
                [% if(CouponInputVisible === true) { %]
                <div class="fullcart-cupon-text-wrap">
                    <span class="fullcart-cupon-text">[%= localize('shoppingCartCuponCode')%] :</span>
                </div>
                <div class="fullcart-cupon-inputs">
                    <div class="input-wrap input-coupon">
                        <input type="text" class="" id="txtCertificateCoupon"></div>
                    <span class="btn-c"><a class="btn btn-action btn-middle" href="javascript:void(0);"
                        data-cart-apply-cert-cupon="#txtCertificateCoupon">[%= localize('shoppingCartAplly')%]</a></span>
                </div>
                [% } %]
            </td>
            <td class="recalc">
                <a href="javascript:void(0);" data-cart-refresh="true" class="link-recal">[%= localize('shoppingCartRecalculate')%]</a>
            </td>
            <td class="fullcart-summary">
                [% for(var i=0, arrLength = Summary.length - 1; i< arrLength; i++) { %]
                <div>
                    <span class="fullcart-summary-text">[%= Summary[i].Key%]: </span><span class="price">
                        [%= Summary[i].Value%]</span></div>
                [% } %]
            </td>
            <td class="empty">
            </td>
        </tr>
    </tbody>
    <tfoot class="fullcart-footer">
        <tr>
            <td colspan="3" class="fullcart-note">
                [%= localize('shoppingCartShippingPriceLater')%] [% if(Valid.length > 0) { %]
                <div id="errorMessage" class="not-available">
                    [%= Valid%]
                </div>
                [% } %]
            </td>
            <td colspan="2" class="fullcart-result">
                <span class="fullcart-summary-text">[%= Summary[Summary.length -1].Key%]: </span>
                <span class="price">[%= Summary[Summary.length -1].Value%]</span>
            </td>
            <td class="empty">
            </td>
        </tr>
    </tfoot>
</table>
[% } %]