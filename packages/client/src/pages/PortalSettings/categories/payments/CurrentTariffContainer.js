import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const StyledCurrentTariffContainer = styled.div`
  display: flex;
  height: 40px;
  background: #f8f9f9;

  margin-bottom: 24px;
  grid-template-columns: repeat(auto, minmax(150px, auto));

  white-space: nowrap;
  grid-gap: 20px;

  p {
    padding: 12px 16px;
  }
`;

const CurrentTariffContainer = ({ quota, style }) => {
  const { t } = useTranslation("Payments");

  const { usersCount, maxUsersCount, storageSize, usedSize } = quota;

  const convertedBytes = (bytes) => {
    const sizes = ["Bytes", "Kb", "Mb", "Gb", "Tb"]; //TODO: need to specify about translation
    if (bytes == 0) return "0 B";

    const i = Math.floor(Math.log(bytes) / Math.log(1024));

    return parseFloat((bytes / Math.pow(1024, i)).toFixed(2)) + " " + sizes[i];
  };

  const storageSizeConverted = convertedBytes(storageSize);
  const usedSizeConverted = convertedBytes(usedSize);

  return (
    <StyledCurrentTariffContainer style={style} className="current-tariff">
      <Text isBold noSelect>
        {t("AddedManagers")}:{" "}
        <Text as="span" isBold>
          {usersCount}/{maxUsersCount}
        </Text>
      </Text>
      <Text isBold noSelect>
        {t("StorageSpace")}:{" "}
        <Text as="span" isBold>
          {usedSizeConverted}/{storageSizeConverted}
        </Text>
      </Text>
    </StyledCurrentTariffContainer>
  );
};

export default inject(({ auth }) => {
  const { quota } = auth;

  return { quota };
})(observer(CurrentTariffContainer));
