import React from "react";
import {
  EmptyScreenContainer,
  IconButton,
  Link,
  Box,
  Grid,
} from "asc-web-components";

const EmptyScreen = ({ t, onResetFilter, isEmptyGroup }) => {
  const title = isEmptyGroup ? "EmptyGroupTitle" : "NotFoundTitle";
  const description = isEmptyGroup
    ? "EmptyGroupDescription"
    : "NotFoundDescription";
  return (
    <EmptyScreenContainer
      imageSrc="images/empty_screen_filter.png"
      imageAlt="Empty Screen Filter image"
      headerText={t(title)}
      descriptionText={t(description)}
      buttons={
        <Grid
          marginProp="13px 0"
          gridColumnGap="8px"
          columnsProp={["12px 1fr"]}
        >
          {isEmptyGroup ? null : (
            <>
              <Box>
                <IconButton
                  className="empty-folder_container-icon"
                  size="12"
                  onClick={onResetFilter}
                  iconName="CrossIcon"
                  isFill
                  color="#657077"
                />
              </Box>{" "}
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
              </Box>{" "}
            </>
          )}
        </Grid>
      }
    />
  );
};

export default EmptyScreen;
