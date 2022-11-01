import { inject, observer } from "mobx-react";
import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
const TelegramConnectionContainer = ({ t, isTelegramConnected }) => {
  const linkText = isTelegramConnected
    ? t("Common:Disconnect")
    : t("Common:Connect");

  return (
    <>
      <Text fontSize="15px" fontWeight="600">
        {t("TelegramConnection")}
      </Text>
      <Link href={"/"}></Link>
    </>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { isTelegramConnected } = targetUserStore;

  return { isTelegramConnected };
})(observer(TelegramConnectionContainer));
