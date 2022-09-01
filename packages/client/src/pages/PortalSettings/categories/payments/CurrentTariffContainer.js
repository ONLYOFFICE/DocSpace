import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const StyledCurrentTariffContainer = styled.div`
  display: flex;
  min-height: 40px;
  background: #f8f9f9;
  margin-bottom: 24px;
  flex-wrap: wrap;
  margin-top: 16px;
  padding: 12px 16px;
  box-sizing: border-box;
  padding-bottom: 0;

  div {
    padding-bottom: 12px;
    margin-right: 24px;
  }

  p {
    margin-bottom: 0;
    .current-tariff_count {
      margin-left: 5px;
    }
  }
`;

const CurrentTariffContainer = ({ quota, portalQuota, style }) => {
  const { t } = useTranslation("Payments");

  const { usedSize } = quota;
  const { maxTotalSize, countRoom } = portalQuota;

  const addedRooms = 1;
  const maxManagers = 50;
  const addedManagers = 5;
  const convertedBytes = (bytes) => {
    const sizeNames = [
      t("Bytes"),
      t("Kilobyte"),
      t("Megabyte"),
      t("Gigabyte"),
      t("Terabyte"),
    ];
    if (bytes == 0) return `${"0" + " " + t("Bytes")}`;

    const i = Math.floor(Math.log(bytes) / Math.log(1024));

    return (
      parseFloat((bytes / Math.pow(1024, i)).toFixed(2)) + " " + sizeNames[i]
    );
  };

  const storageSizeConverted = convertedBytes(maxTotalSize);
  const usedSizeConverted = convertedBytes(usedSize);

  return (
    <StyledCurrentTariffContainer style={style}>
      <div>
        <Text isBold noSelect>
          {t("Room")}:
          <Text className="current-tariff_count" as="span" isBold>
            {countRoom > 10000 ? addedRooms : addedRooms + "/" + countRoom}
          </Text>
        </Text>
      </div>
      <div>
        <Text isBold noSelect>
          {t("AddedManagers")}:
          <Text className="current-tariff_count" as="span" isBold>
            {addedManagers}/{maxManagers}
          </Text>
        </Text>
      </div>
      <div>
        <Text isBold noSelect>
          {t("StorageSpace")}:
          <Text className="current-tariff_count" as="span" isBold>
            {usedSizeConverted}/{storageSizeConverted}
          </Text>
        </Text>
      </div>
    </StyledCurrentTariffContainer>
  );
};

export default inject(({ auth }) => {
  const { quota, portalQuota } = auth;

  return { quota, portalQuota };
})(observer(CurrentTariffContainer));
