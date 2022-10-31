import { inject, observer } from "mobx-react";
import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
const UsefulTipsContainer = ({
  t,
  tempTipsSubscription,
  setTempTipsSubscription,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    setTempTipsSubscription(checked);
  };

  return (
    <>
      <Text fontSize="15px" fontWeight="600">
        {t("UsefulTips")}
      </Text>
      <ToggleButton
        className="toggle-btn"
        label={t("Common:Email")}
        onChange={onChangeEmailSubscription}
        isChecked={tempTipsSubscription}
      />
    </>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { tempTipsSubscription, setTempTipsSubscription } = targetUserStore;

  return { tempTipsSubscription, setTempTipsSubscription };
})(observer(UsefulTipsContainer));
