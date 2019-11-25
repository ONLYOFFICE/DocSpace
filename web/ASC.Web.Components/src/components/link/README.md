# Link

It is a link with 2 types:

1. page - simple link which refer to other pages and parts of current page;
2. action - link, which usually hasn`t hyperlink and do anything on click - open dropdown, filter data, etc

### Usage

```js
import { Link } from "asc-web-components";
```

```jsx
<Link 
  type="page" 
  color="black" 
  href="https://github.com" 
  isBold={true}
>
  Bold page link
</Link>
```

### Properties

| Props               |   Type   | Required |                  Values                  |        Default        | Description                                                                                    |
| ------------------- | :------: | :------: | :--------------------------------------: | :-------------------: | ---------------------------------------------------------------------------------------------- |
| `type`              | `oneOf`  |    -     |             `action`, `page`             |        `page`         | Type of link                                                                                   |
| `color`             | `string` |    -     |                    -                     |       `#333333`       | Color of link                                                                                  |
| `fontSize`          | `number` |    -     |                    -                     |         `13`          | Font size of link (in px)                                                                      |
| `href`              | `string` |    -     |                    -                     |      `undefined`      | Used as HTML `href` property                                                                   |
| `isBold`            |  `bool`  |    -     |                    -                     |        `false`        | Set font weight                                                                                |
| `title`             | `string` |    -     |                    -                     |           -           | Used as HTML `title` property                                                                  |
| `target`            | `oneOf`  |    -     | `\_blank`, `\_self`, `\_parent`, `\_top` |           -           | The _target_ attribute specifies where the linked document will open when the link is clicked. |  |
| `isTextOverflow`    |  `bool`  |    -     |                    -                     |        `true`         | Activate or deactivate _text-overflow_ CSS property with ellipsis (' … ') value                |
| `isHovered`         |  `bool`  |    -     |                    -                     |        `false`        | Set hovered state and effects of link.                                                         |
| `isSemitransparent` |  `bool`  |    -     |                    -                     |        `false`        | Set css-property 'opacity' to 0.5. Usually apply for users with "pending" status               |  |
| `onClick`           |  `func`  |    -     |                    -                     |           -           | What the link will trigger when clicked. Only for \'action\' type of link                      |
| `rel`               | `string` |    -     |                    -                     | `noopener noreferrer` | Used as HTML `rel` property                                                                    |
| `tabIndex`          | `number` |    -     |                    -                     |         `-1`          | Used as HTML `tabindex` property                                                               |
