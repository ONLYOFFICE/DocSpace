# HelpButton

HelpButton is used for a action on a page

### Usage

```js
import { HelpButton } from "asc-web-components";
```

```jsx
<HelpButton tooltipContent={<Text.Body>Tooltip content</Text.Body>} />
```

#### Usage with link

```jsx
<HelpButton
  tooltipContent={
    <Text.Body>
      Tooltip content with{" "}
      <Link isHovered={true} href="/">
        link
      </Link>
    </Text.Body>
  }
/>
```

#### Usage with aside

```jsx
<HelpButton
  tooltipContent={
    <>
      <p>This is a global react component tooltip</p>
      <p>You can put every thing here</p>
      <ul>
        <li>Word</li>
        <li>Chart</li>
        <li>Else</li>
      </ul>
    </>
  }
/>
```

### Properties

| Props                     |       Type        | Required |              Values              | Default | Description                                      |
| ------------------------- | :---------------: | :------: | :------------------------------: | :-----: | ------------------------------------------------ |
| `tooltipContent`          | `object`,`string` |    ✅    |                -                 |    -    | Tooltip content                                  |
| `place`                   |     `string`      |    -     | `top`, `right`, `bottom`, `left` |  `top`  | Tooltip placement                                |
| `displayType`             |      `oneOf`      |    -     |   `dropdown`, `aside`, `auto`    | `auto`  | Tooltip display type                             |
| `helpButtonHeaderContent` |     `string`      |    -     |                -                 |    -    | Tooltip header content (tooltip opened in aside) |
| `className`               |     `string`      |    -     |                -                 |    -    | Set component class                              |
