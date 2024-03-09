# Random Texture Stitcher Proof of Concept
This program takes an image with 16 textures, and randomly creates a texture of a given size.
There are 2 base textures, texture 0000 (top left) and texture 1111 (bottom right).
The ID describes the seams of the texture starting left and going counter-clockwise.
For example a texture with the ID 1010 would connect to texture 0000 at the bottom and top and connect to 1111 at the sides.

## An image with these textures could look like this:
![Input Example](/images/test.png)

## The result will look something like this:
![Output Example](/images/out.png)

> Note how sides of all squares connect with the same color, but the squares themselves are randomly placed.