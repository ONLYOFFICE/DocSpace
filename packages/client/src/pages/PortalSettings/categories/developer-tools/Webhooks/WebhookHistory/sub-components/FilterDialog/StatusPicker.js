import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { Text } from "@docspace/components";
import { Button } from "@docspace/components";

const RoundedButton = styled(Button)`
  font-size: 13px;
  font-weight: 400;

  border-radius: 16px;
  margin-right: 8px;
`;

const StatusPicker = ({ Selectors, toggleStatus, isStatusSelected }) => {
  const handleStatusClick = (e) => toggleStatus(e.target.textContent);
  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        Status
      </Text>
      <Selectors>
        <RoundedButton
          label="Not sent"
          onClick={handleStatusClick}
          primary={isStatusSelected("Not sent")}
        />
        <RoundedButton label="2XX" onClick={handleStatusClick} primary={isStatusSelected("2XX")} />
        <RoundedButton label="3XX" onClick={handleStatusClick} primary={isStatusSelected("3XX")} />
        <RoundedButton label="4XX" onClick={handleStatusClick} primary={isStatusSelected("4XX")} />
        <RoundedButton label="5XX" onClick={handleStatusClick} primary={isStatusSelected("5XX")} />
      </Selectors>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleStatus, isStatusSelected } = webhooksStore;

  return { toggleStatus, isStatusSelected };
})(observer(StatusPicker));
