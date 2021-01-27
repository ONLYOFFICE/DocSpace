import React from "react";
import {
  EmptyScreenContainer,
  IconButton,
  Link,
  Box,
  Grid,
} from "asc-web-components";

class EmptyScreen extends React.Component {
  render() {
    const { t, onResetFilter, groupId } = this.props;
    return (
      <EmptyScreenContainer
        imageSrc="images/empty_screen_filter.png"
        imageAlt="Empty Screen Filter image"
        headerText={t("NotFoundTitle")}
        descriptionText={t("NotFoundDescription")}
        buttons={
          <Grid
            marginProp="13px 0"
            gridColumnGap="8px"
            columnsProp={["12px 1fr"]}
          >
            <Box>
              <IconButton
                className="empty-folder_container-icon"
                size="12"
                onClick={onResetFilter}
                iconName="CrossIcon"
                isFill
                color="#657077"
              />
            </Box>
            <Box marginProp="-4px 0 0 0">
              <Link
                type="action"
                isHovered={true}
                fontWeight="600"
                color="#555f65"
                onClick={onResetFilter}
              >
                {t("ClearButton")}
              </Link>
            </Box>
          </Grid>
        }
      />
    );
  }
}

export default EmptyScreen;
