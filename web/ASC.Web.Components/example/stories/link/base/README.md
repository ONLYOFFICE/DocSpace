# Link

## Usage

```js
import { Link } from 'asc-web-components';
```

#### Description

It is a link with 2 types:  
1) page - simple link which refer to other pages and parts of current page;  
2) action - link, which usually hasn`t hyperlink and do anything on click - open dropdown, filter data, etc

#### Usage

```js
<Link type = "page" color = "black" href="https://github.com" isBold = {true}>Bold page link</Link>
```

#### Properties (common)

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `type`             | `oneOf`  |    -     | action, page                | `page`        | Type of link                         |
| `color`            | `oneOf`  |    -     | gray, black, blue| `black`  | Color of link in all states - hover, active, visited              |
| `fontSize`           | `number`   |    -     | -                       | `12`        | Font size of link (in px)                        |
| `href`           | `string`   |    -     | -                           | `undefined`        | Hyperlink, usually used in *page* type             |
| `isBold`           | `bool`   |    -     | -                         | `false`        | Set font weight                          |
| `title`           | `string`   |    -     | -                           | -        | Title of link                          |
| `target`           | `oneOf`   |    -     | _blank, _self, _parent, _top   | -    | The *target* attribute specifies where the linked document will open when the link is clicked.                          |                      |
| `isTextOverflow`   | `bool`   |    -     | -                           | `true`        |Activate or deactivate *text-overflow* CSS property with ellipsis (' … ') value                           |
| `isHovered`           | `bool`   |    -     | -                           | `false`        | Show hovered state of link. Only for demo        |
        |

#### Properties (only for \'action\' type of link)

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `dropdownType`       | `oneOf`  |    -     | alwaysDotted, appearDottedAfterHover, none                   | `none`        | Type of dropdown: *none* is neither, *alwaysDotted* is always show dotted style and icon of dropdown, *appearDottedAfterHover* is show dotted style and icon of dropdown only after hover       |
| `onClick`           | `func`   |    -     | -                           | -        | What the link will trigger when clicked - open/close dropdown, filter, popup, etc

