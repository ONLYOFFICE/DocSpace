# LinkWithDropdown

Link with dropdown

### Usage

```js
import LinkWithDropdown from "@docspace/components/link-with-dropdown";
```

```jsx
<LinkWithDropdown color="black" isBold={true} data={data}>
  Link with dropdown
</LinkWithDropdown>
```

### Properties

| Props               |       Type       | Required |                 Values                 | Default | Description                                                                                                                                               |
| ------------------- | :--------------: | :------: | :------------------------------------: | :-----: | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `color`             |     `oneOf`      |    -     |        `gray`, `black`, `blue`         | `black` | Color of link in all states - hover, active, visited                                                                                                      |
| `data`              |     `array`      |    -     |                   -                    |    -    | Array of objects, each can contain `<DropDownItem />` props                                                                                               |
| `dropdownType`      |     `oneOf`      |    ✅    | `alwaysDashed, appearDashedAfterHover` |    -    | Type of dropdown: alwaysDashed is always show dotted style and icon of arrow, appearDashedAfterHover is show dotted style and icon arrow only after hover |
| `fontSize`          |     `string`     |    -     |                   -                    | `13px`  | Font size of link                                                                                                                                         |
| `fontWeight`        | `number, string` |    -     |                   -                    |    -    | Font weight of link                                                                                                                                       |
| `id`                |     `string`     |    -     |                   -                    |    -    | Accepts id                                                                                                                                                |
| `isBold`            |      `bool`      |    -     |                   -                    | `false` | Set font weight                                                                                                                                           |
| `isSemitransparent` |      `bool`      |    -     |                   -                    | `false` | Set css-property 'opacity' to 0.5. Usually apply for users with "pending" status                                                                          |  |
| `isTextOverflow`    |      `bool`      |    -     |                   -                    | `true`  | Activate or deactivate _text-overflow_ CSS property with ellipsis (' … ') value                                                                           |
| `style`             |  `obj`, `array`  |    -     |                   -                    |    -    | Accepts css style                                                                                                                                         |
| `title`             |     `string`     |    -     |                   -                    |    -    | Title of link                                                                                                                                             |  |
