import EmptyScreenPersonSvgUrl from "PUBLIC_DIR/images/empty_screen_persons.svg?url";
import EmptyScreenPersonSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_persons_dark.svg?url";
import EmptyScreenAltSvgUrl from "PUBLIC_DIR/images/empty_screen_alt.svg?url";
import EmptyScreenAltSvgDarkUrl from "PUBLIC_DIR/images/empty_screen_alt_dark.svg?url";

import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import Text from "@docspace/components/text";
import { StyledSeveralItemsContainer } from "../../styles/severalItems";

const SeveralItems = ({ isAccounts, theme, selectedItems }) => {
  const { t } = useTranslation("InfoPanel");

  const emptyScreenAlt = theme.isBase
    ? EmptyScreenAltSvgUrl
    : EmptyScreenAltSvgDarkUrl;

  const emptyScreenPerson = theme.isBase
    ? EmptyScreenPersonSvgUrl
    : EmptyScreenPersonSvgDarkUrl;

  const imgSrc = isAccounts ? emptyScreenPerson : emptyScreenAlt;

  const itemsText = isAccounts
    ? t("InfoPanel:SelectedUsers")
    : t("InfoPanel:ItemsSelected");

  return (
    <StyledSeveralItemsContainer
      isAccounts={isAccounts}
      className="no-thumbnail-img-wrapper"
    >
      <img src={imgSrc} />
      <Text fontSize="16px" fontWeight={700}>
        {`${itemsText}: ${selectedItems.length}`}
      </Text>
    </StyledSeveralItemsContainer>
  );
};

export default inject(({ auth }) => {
  const selectedItems = auth.infoPanelStore.getSelectedItems();

  return {
    theme: auth.settingsStore.theme,
    selectedItems,
  };
})(observer(SeveralItems));
