# EmptyScreenContainer

## Usage

```js
import { EmptyScreenContainer } from 'asc-web-components';
```

#### Description

Used to display empty screen page

#### Usage

```js
<EmptyScreenContainer
      imageSrc={text("imageSrc", "empty_screen_filter.png")}
      imageAlt={text("imageAlt", "Empty Screen Filter image")}
      headerText={text(
        "headerText",
        "No results matching your search could be found"
      )}
      descriptionText={text(
        "descriptionText",
        "No people matching your filter can be displayed in this section. Please select other filter options or clear filter to view all the people in this section."
      )}
      buttons={
        <>
          <Icons.CrossIcon size="small" style={{marginRight: "4px"}} />
          <Link
            type="action"
            isHovered={true}
            onClick={(e) => action("Reset filter clicked")(e)}
          >
            Reset filter
          </Link>
        </>
      }
    />
```

#### Properties

| Props              | Type                  | Required | Values                         | Default         | Description                                                                                          |
| ------------------ | ----------------------| :------: | ---------------------------    | --------------- |----------------------------------------------------------------------------------------------------- |
| `imageSrc`            | `string`              |    -     | -                              | -       | Image url source                                                                                           |
| `imageAlt`             | `string`|    -     | -                              |   -          | Alternative image text                                                                        |
| `headerText`       | `string`                |    -     | -                              | -        | Header text                                               |
| `descriptionText`         | `string`              |    -    | -                              | - | Description text                                                                                            |
| `buttons`           | `element(s)`                |    -     | -                              | -          | Content of EmptyContentButtonsContainer                                                                    |