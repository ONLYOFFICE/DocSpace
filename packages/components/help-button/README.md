# HelpButton

HelpButton is used for a action on a page

### Usage

```js
import HelpButton from "@docspace/components/help-button";
```

```jsx
<HelpButton tooltipContent={<Text>Tooltip content</Text>} />
```

#### Usage with link

```jsx
<HelpButton
  tooltipContent={
    <Text>
      Tooltip content with{" "}
      <Link isHovered={true} href="/">
        link
      </Link>
    </Text>
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

| Props                     |   Type   | Required |           Values            | Default | Description                                      |
| ------------------------- | :------: | :------: | :-------------------------: | :-----: | ------------------------------------------------ |
| `className`               | `string` |    -     |              -              |    -    | Accepts class                                    |
| `displayType`             | `oneOf`  |    -     | `dropdown`, `aside`, `auto` | `auto`  | Tooltip display type                             |
| `helpButtonHeaderContent` | `string` |    -     |              -              |    -    | Tooltip header content (tooltip opened in aside) |  | `id` | `string` | - | - | - | Accepts id |
