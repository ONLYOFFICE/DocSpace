import Tooltip from "@docspace/components/tooltip";
import LoadErrorIcon from "PUBLIC_DIR/images/load.error.react.svg";
import styled from "styled-components";
import Text from "@docspace/components/text";
import React from "react";
import { useTheme } from "styled-components";

const StyledLoadErrorIcon = styled(LoadErrorIcon)`
  outline: none !important;
`;

const ErrorFileUpload = ({ t, item, onTextClick, showPasswordInput }) => {
  const { interfaceDirection } = useTheme();
  const placeTooltip = interfaceDirection === "rtl" ? "right" : "left";
  return (
    <>
      <div className="upload_panel-icon">
        <StyledLoadErrorIcon
          size="medium"
          data-for="errorTooltip"
          data-tip={item.error || t("Common:UnknownError")}
        />
        <Tooltip
          id="errorTooltip"
          offsetTop={0}
          getContent={dataTip => (
            <Text fontSize="13px" noSelect>
              {dataTip}
            </Text>
          )}
          effect="float"
          place={placeTooltip}
          maxWidth="320"
          color="#f8f7bf"
        />
        {item.needPassword && (
          <Text
            className="enter-password"
            fontWeight="600"
            color="#A3A9AE"
            onClick={onTextClick}>
            {showPasswordInput ? t("HideInput") : t("EnterPassword")}
          </Text>
        )}
      </div>
    </>
  );
};
export default ErrorFileUpload;
