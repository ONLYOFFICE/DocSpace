import EmptyScreenPersonSvgUrl from "PUBLIC_DIR/images/empty_screen_persons.svg?url";
import EmptyScreenPersonSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_persons_dark.svg?url";
import ClearEmptyFilterSvgUrl from "PUBLIC_DIR/images/clear.empty.filter.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import IconButton from "@docspace/components/icon-button";
import Link from "@docspace/components/link";
import Box from "@docspace/components/box";
import Grid from "@docspace/components/grid";

const EmptyScreen = ({ resetFilter, setIsLoading, theme }) => {
  const { t } = useTranslation(["People", "Common"]);

  const title = t("NotFoundUsers");
  const description = t("NotFoundUsersDescription");

  const onResetFilter = () => {
    setIsLoading(true);
    resetFilter();
  };

  const imageSrc = theme.isBase
    ? EmptyScreenPersonSvgUrl
    : EmptyScreenPersonSvgDarkUrl;
  return (
    <>
      <EmptyScreenContainer
        imageSrc={imageSrc}
        imageAlt="Empty Screen Filter image"
        headerText={title}
        descriptionText={description}
        buttons={
          <Grid
            marginProp="13px 0"
            gridColumnGap="8px"
            columnsProp={["12px 1fr"]}
          >
            {
              <>
                <Box>
                  <IconButton
                    className="empty-folder_container-icon"
                    size="12"
                    onClick={onResetFilter}
                    iconName={ClearEmptyFilterSvgUrl}
                    isFill
                  />
                </Box>
                <Box marginProp="-4px 0 0 0">
                  <Link
                    type="action"
                    isHovered={true}
                    fontWeight="600"
                    onClick={onResetFilter}
                  >
                    {t("Common:ClearFilter")}
                  </Link>
                </Box>
              </>
            }
          </Grid>
        }
      />
    </>
  );
};

export default inject(({ auth, peopleStore, clientLoadingStore }) => {
  const { resetFilter } = peopleStore;

  const { setIsSectionBodyLoading } = clientLoadingStore;

  const setIsLoading = (param) => {
    setIsSectionBodyLoading(param);
  };
  return {
    resetFilter,

    setIsLoading,
    theme: auth.settingsStore.theme,
  };
})(observer(EmptyScreen));
