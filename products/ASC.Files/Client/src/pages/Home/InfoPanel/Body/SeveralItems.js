import Text from "@appserver/components/text";
import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import { StyledTitle } from "./styles/styles.js";

const SeveralItems = (props) => {
    const { t, selectedItems, getIcon } = props;

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
                    className="no-thumbnail-img"
                    src="images/empty_screen.png"
                />
            </div>
        </>
    );
};

export default inject(({}) => {
    return {};
})(withTranslation(["InfoPanel"])(observer(SeveralItems)));
