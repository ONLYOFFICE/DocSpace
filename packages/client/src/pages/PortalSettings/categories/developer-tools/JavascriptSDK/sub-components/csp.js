import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";

import TextInput from "@docspace/components/text-input";
import HelpButton from "@docspace/components/help-button";
import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import SelectedItem from "@docspace/components/selected-item";

const CategoryHeader = styled.div`
  margin-top: 24px;
  margin-bottom: 16px;
  font-size: 16px;
  font-style: normal;
  font-weight: 700;
  line-height: 22px;
`;

const Container = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 16px;
`;

const ChipsContainer = styled.div`
  display: flex;
  align-items: center;
  gap: 8px 4px;
  margin-bottom: 16px;
  flex-wrap: wrap;
`;

const getChips = (domains) =>
  domains ? (
    domains.map((item) => (
      <SelectedItem isInline text={item} onClose={() => {}} />
    ))
  ) : (
    <></>
  );

const CSP = ({ t, cspSettings, getCSPSettings, setCSPSettings }) => {
  useEffect(() => {
    !cspSettings && getCSPSettings();
  }, [cspSettings?.domains]);

  const [domains, setDomains] = useState(cspSettings?.domains);

  return (
    <>
      <CategoryHeader>{t("CSPHeader")}</CategoryHeader>
      <Container>
        {t("CSPDescription")}
        <HelpButton
          offsetRight={0}
          size={12}
          tooltipContent={<Text fontSize="12px">{t("CSPHelp")}</Text>}
        />
      </Container>
      <Container>
        <TextInput onChange={() => {}} placeholder={t("CSPInputPlaceholder")} />
        <SelectorAddButton onClick={() => {}} />
      </Container>
      <ChipsContainer>{getChips(domains)}</ChipsContainer>
    </>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { cspSettings, getCSPSettings, setCSPSettings } = settingsStore;
  return { cspSettings, getCSPSettings, setCSPSettings };
})(observer(CSP));
