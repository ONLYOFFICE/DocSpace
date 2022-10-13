import React from "react";
import styled from "styled-components";

import ToggleParam from "../Params/ToggleParam";
import PrivacyLimitationsWarning from "./PrivacyLimitationsWarning";

const StyledIsPrivateParam = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;
`;

const IsPrivateParam = ({ t, isPrivate, onChangeIsPrivate }) => {
  return (
    <StyledIsPrivateParam>
      <ToggleParam
        title={t("MakeRoomPrivateTitle")}
        description={t("MakeRoomPrivateDescription")}
        isChecked={isPrivate}
        onCheckedChange={onChangeIsPrivate}
      />
      {isPrivate && <PrivacyLimitationsWarning t={t} />}
    </StyledIsPrivateParam>
  );
};

export default IsPrivateParam;
