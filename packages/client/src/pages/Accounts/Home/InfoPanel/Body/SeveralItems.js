import Text from "@docspace/components/text";
import React from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

const SeveralItems = (props) => {
  const { t, selectedItems, getIcon, getFolderInfo } = props;
  const itemsIcon = getIcon(24, ".file");

  return (
    <>
      <StyledTitle>
        <ReactSVG className="icon" src={itemsIcon} />
        <Text className="text" fontWeight={600} fontSize="16px">
          {`${t("ItemsSelected")}: ${selectedItems.length}`}
        </Text>
      </StyledTitle>

      <div className="no-thumbnail-img-wrapper">
        <img
          size="96px"
          className="no-thumbnail-img"
          src="images/empty_screen.png"
        />
      </div>
    </>
  );
};

export default withTranslation(["InfoPanel"])(SeveralItems);
