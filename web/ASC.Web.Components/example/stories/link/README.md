# Link

## Usage

```js
import { Link } from 'asc-web-components';
```

#### Description

It is a link with 2 types:  
1) page - simple link which refer to other pages and parts of current page;  
2) action - link, which usually hasn`t hyperlink and do anything on click - open dropdown, filter data. etc

#### Usage

```js
<Link type = "page" color = "black" href="https://github.com" isBold = {true}>Bold page link</Link>
```

#### Properties (common)

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `type`             | `oneOf`  |    -     | action, page                | `page`        | Type of link                         |
| `color`            | `oneOf`  |    -     | gray, black, blue, filter, profile| `black`  | Color of link in all states - hover, active, visited              |
| `fontSize`           | `string`   |    -     | -                       | `12px`        | Font size of link                         |
| `href`           | `string`   |    -     | -                           | `javascript:void(0)`        | Hyperlink, usually used in *page* type             |
| `isBold`           | `bool`   |    -     | -                         | `false`        | Set font weight                          |
| `title`           | `string`   |    -     | -                           | -        | Title of link                          |
| `target`           | `oneOf`   |    -     | _blank, _self, _parent, _top   | -    | The *target* attribute specifies where the linked document will open when the link is clicked.                          |
| `rel`           | `string`   |    -     | -                           | 'noopener noreferrer' if target === '_blank'      | The *rel* attribute specifies the relationship between the current document and the linked document. Only used if the href attribute is present.                          |
| `isTextOverflow`   | `bool`   |    -     | -                           | `true`        |Activate or deactivate *text-overflow* CSS property with ellipsis (' … ') value                           |
| `isHovered`           | `bool`   |    -     | -                           | `false`        | Show hovered state of link. Only for demo        |
| `onClick`           | `func`   |    -     | -                           | -        | What the link will trigger when clicked. Usually use in *action* type         |

#### Properties (only for \'action\' type of link)

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `isDotted`           | `bool`   |    -     | -                           | -       | Add dots decoration under link in usual and visited state |
| `isHoverDotted`      | `bool`   |    -     | -                   | -        | Add dots decoration under link in hovered state      |
| `isDropdown`         | `bool`   |    -     | -                           | -        | Add dropdown        |
| `dropdownType`       | `oneOf`  |    -     | filter, menu, none                   | `none`        | Type of dropdown        |
| `dropdownColor`      | `oneOf`  |    -     | filter, profile, sorting,number,email, group| `filter` | Color of dropdown        |
| `dropdownRightIndent`           | `string`   |    -     | -                           | `-10px`        | The right property affects the horizontal position of a dropdown        |
| `displayDropdownAfterHover`           | `bool`   |    -     | -                          | `false`        | Set appearance dropdown icon when hover        |
