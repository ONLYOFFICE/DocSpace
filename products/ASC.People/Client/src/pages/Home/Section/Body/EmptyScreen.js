import React from "react";

import EmptyScreenContainer from "@appserver/components/empty-screen-container";
import IconButton from "@appserver/components/icon-button";
import Link from "@appserver/components/link";
import Box from "@appserver/components/box";
import Grid from "@appserver/components/grid";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const EmptyScreen = ({ resetFilter, isEmptyGroup, setIsLoading }) => {
  const { t } = useTranslation(["Home", "Common"]);

  const title = isEmptyGroup ? t("EmptyGroupTitle") : t("NotFoundTitle");
  const description = isEmptyGroup
    ? t("EmptyGroupDescription")
    : t("NotFoundDescription");

  const onResetFilter = () => {
    setIsLoading(true);
    resetFilter(true).finally(() => setIsLoading(false));
  };

  return (
    <EmptyScreenContainer
      imageSrc="images/empty_screen_filter.png"
      imageAlt="Empty Screen Filter image"
      headerText={title}
      descriptionText={description}
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
                  {t("Common:ClearButton")}
                </Link>
              </Box>{" "}
            </>
          )}
        </Grid>
      }
    />
  );
};

export default inject(({ peopleStore }) => {
  const { loadingStore, resetFilter, selectedGroupStore } = peopleStore;
  const { isEmptyGroup } = selectedGroupStore;
  const { setIsLoading } = loadingStore;
  return {
    resetFilter,
    isEmptyGroup,
    setIsLoading,
  };
})(observer(EmptyScreen));
