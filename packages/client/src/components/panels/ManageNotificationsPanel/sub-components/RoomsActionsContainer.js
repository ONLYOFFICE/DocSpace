import { inject, observer } from "mobx-react";
import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
const RoomsActionsContainer = ({ t }) => {
  return (
    <Text fontSize="15px" fontWeight="600">
      {t("RoomsActions")}
    </Text>
  );
};

export default inject(({}) => {
  return {};
})(observer(RoomsActionsContainer));
