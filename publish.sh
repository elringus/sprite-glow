#!/usr/bin/env sh

# abort on errors
set -e

cd Assets/SpriteGlow

git init
git add -A
git commit -m 'publish'
git push -f git@github.com:Elringus/SpriteGlow.git master:package

cd -