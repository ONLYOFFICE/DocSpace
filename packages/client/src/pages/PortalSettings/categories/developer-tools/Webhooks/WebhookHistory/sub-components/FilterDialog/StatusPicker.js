import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { useTranslation } from "react-i18next";

const RoundedButton = styled(Button)`
  box-sizing: border-box;
  font-size: 13px;
  font-weight: 400;
  padding: 13.5px 15px;

  border-radius: 16px;
  margin-right: 7px;

  line-height: 20px;
`;

const StatusPicker = ({ Selectors, filters, setFilters }) => {
  const { t } = useTranslation(["Webhooks", "People"]);

  const StatusCodes = {
    0: "Not sent",
    200: "2XX",
    300: "3XX",
    400: "4XX",
    500: "5XX",
  };

  const isStatusSelected = (statusCode) => {
    return filters.status.includes(statusCode);
  };
  const handleStatusClick = (statusCode) => {
    setFilters((prevFilters) => ({
      ...prevFilters,
      status: prevFilters.status.includes(statusCode)
        ? prevFilters.status.filter((statusItem) => statusItem !== statusCode)
        : [...prevFilters.status, statusCode],
    }));
  };
  return (
    <>
      <Text fontWeight={600} fontSize="15px">
        {t("People:UserStatus")}
      </Text>
      <Selectors>
        <RoundedButton
          label={t("NotSent")}
          onClick={() => handleStatusClick(StatusCodes[0])}
          primary={isStatusSelected(StatusCodes[0])}
        />
        <RoundedButton
          label={StatusCodes[200]}
          onClick={() => handleStatusClick(StatusCodes[200])}
          primary={isStatusSelected(StatusCodes[200])}
        />
        <RoundedButton
          label={StatusCodes[300]}
          onClick={() => handleStatusClick(StatusCodes[300])}
          primary={isStatusSelected(StatusCodes[300])}
        />
        <RoundedButton
          label={StatusCodes[400]}
          onClick={() => handleStatusClick(StatusCodes[400])}
          primary={isStatusSelected(StatusCodes[400])}
        />
        <RoundedButton
          label={StatusCodes[500]}
          onClick={() => handleStatusClick(StatusCodes[500])}
          primary={isStatusSelected(StatusCodes[500])}
        />
      </Selectors>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const {} = webhooksStore;

  return {};
})(observer(StatusPicker));
