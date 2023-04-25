import React from "react";
import styled from "styled-components";

import { Text } from "@docspace/components";
import { Button } from "@docspace/components";

const RoundedButton = styled(Button)`
  font-size: 13px;
  font-weight: 400;

  border-radius: 16px;
  margin-right: 8px;
`;

export const StatusPicker = ({ Selectors, filterSettings, setFilterSettings }) => {
  const toggleStatus = (e) => {
    const statusName = e.target.textContent;
    if (filterSettings.status.includes(statusName)) {
      setFilterSettings((prevFilterSetting) => ({
        ...prevFilterSetting,
        status: prevFilterSetting.status.filter((statusItem) => statusItem !== statusName),
      }));
    } else {
      setFilterSettings((prevFilterSetting) => ({
        ...prevFilterSetting,
        status: [...prevFilterSetting.status, statusName],
      }));
    }
  };

  const isStatusSelected = (statusName) => {
    return filterSettings.status.includes(statusName);
  };
  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        Status
      </Text>
      <Selectors>
        <RoundedButton
          label="Not sent"
          onClick={toggleStatus}
          primary={isStatusSelected("Not sent")}
        />
        <RoundedButton label="2XX" onClick={toggleStatus} primary={isStatusSelected("2XX")} />
        <RoundedButton label="3XX" onClick={toggleStatus} primary={isStatusSelected("3XX")} />
        <RoundedButton label="4XX" onClick={toggleStatus} primary={isStatusSelected("4XX")} />
        <RoundedButton label="5XX" onClick={toggleStatus} primary={isStatusSelected("5XX")} />
      </Selectors>
    </>
  );
};
