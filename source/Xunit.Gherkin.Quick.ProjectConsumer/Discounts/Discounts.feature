Feature: Discounts
	In order to reward our loyal customers
	As an online shop owner
	I want to give customers discounts and manage them

Scenario Outline: All Deals
Given items like <items> costing <price> in John's cart
  And a <discount> discount
 When John proceeds to checkout
 Then he should only pay <final> for the items in his cart

Examples: Daily deals
  Daily deals are always 17% discounts.
  | items                          | price  | discount | final  |
  | "Writing Great Specifications" | $44.99 | 17%      | $37.34 |

Examples: Coupons
  There are five different types of coupons available.
  | items                          | price  | discount | final  |
  | "Writing Great Specifications" | $44.99 | 5%       | $42.74 |
  | "Writing Great Specifications" | $44.99 | 15%      | $38.24 |
  | "Writing Great Specifications" | $44.99 | 30%      | $31.49 |
  | "Writing Great Specifications" | $44.99 | 50%      | $22.50 |
  | "Writing Great Specifications" | $44.99 | 75%      | $11.25 |

Examples: Bundles
  We offer "buy two, pay for one" bundles.
  | items                             | price  | discount | final  |
  | "Specification by Example" bundle | $44.99 | 50%      | $22.50 |`