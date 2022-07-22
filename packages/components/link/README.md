# Link

It is a link with 2 types:

1. page - simple link which refer to other pages and parts of current page;
2. action - link, which usually hasn`t hyperlink and do anything on click - open dropdown, filter data, etc

### Usage

```js
import Link from "@docspace/components/link";
```

```jsx
<Link type="page" color="black" href="https://github.com" isBold={true}>
  Bold page link
</Link>
```

### Properties

| Props               |       Type       | Required |                  Values                  |        Default        | Description                                                                                    |
| ------------------- | :--------------: | :------: | :--------------------------------------: | :-------------------: | ---------------------------------------------------------------------------------------------- |
| `className`         |     `string`     |    -     |                    -                     |           -           | Accepts class                                                                                  |
| `color`             |     `string`     |    -     |                    -                     |       `#333333`       | Color of link                                                                                  |
| `fontSize`          |     `string`     |    -     |                    -                     |        `13px`         | Font size of link                                                                              |
| `fontWeight`        | `number, string` |    -     |                    -                     |           -           | Font weight of link                                                                            |
| `href`              |     `string`     |    -     |                    -                     |      `undefined`      | Used as HTML `href` property                                                                   |
| `id`                |     `string`     |    -     |                    -                     |           -           | Accepts id                                                                                     |
| `isBold`            |      `bool`      |    -     |                    -                     |        `false`        | Set font weight                                                                                |
| `isHovered`         |      `bool`      |    -     |                    -                     |        `false`        | Set hovered state and effects of link.                                                         |
| `isSemitransparent` |      `bool`      |    -     |                    -                     |        `false`        | Set css-property 'opacity' to 0.5. Usually apply for users with "pending" status               |
| `isTextOverflow`    |      `bool`      |    -     |                    -                     |        `true`         | Activate or deactivate _text-overflow_ CSS property with ellipsis (' … ') value                |
| `noHover`           |      `bool`      |    -     |                    -                     |        `false`        | Disabled hover styles                                                                          |
| `onClick`           |      `func`      |    -     |                    -                     |           -           | What the link will trigger when clicked. Only for \'action\' type of link                      |
| `rel`               |     `string`     |    -     |                    -                     | `noopener noreferrer` | Used as HTML `rel` property                                                                    |
| `style`             |  `obj`, `array`  |    -     |                    -                     |           -           | Accepts css style                                                                              |
| `tabIndex`          |     `number`     |    -     |                    -                     |         `-1`          | Used as HTML `tabindex` property                                                               |
| `target`            |     `oneOf`      |    -     | `\_blank`, `\_self`, `\_parent`, `\_top` |           -           | The _target_ attribute specifies where the linked document will open when the link is clicked. |
| `title`             |     `string`     |    -     |                    -                     |           -           | Used as HTML `title` property                                                                  |
| `type`              |     `oneOf`      |    -     |             `action`, `page`             |        `page`         | Type of link s                                                                                 |
