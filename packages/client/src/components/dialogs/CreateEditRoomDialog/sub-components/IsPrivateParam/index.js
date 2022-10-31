import React from "react";
import styled from "styled-components";

import ToggleParam from "../Params/ToggleParam";

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
    </StyledIsPrivateParam>
  );
};

export default IsPrivateParam;
