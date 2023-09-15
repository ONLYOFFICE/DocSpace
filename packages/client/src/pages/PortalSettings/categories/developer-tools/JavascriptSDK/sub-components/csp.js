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

const CSP = ({ t, cspDomains, getCSPSettings, setCSPSettings }) => {
  useEffect(() => {
    getCSPSettings();
  }, []);

  const [domain, changeDomain] = useState("");

  const onKeyPress = (e) => {
    if (e.key === "Enter" && !!domain.length) {
      addDomain();
    }
  };

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  const getChips = (domains) =>
    domains ? (
      domains.map((item, index) => (
        <SelectedItem
          key={`${item}-${index}`}
          isInline
          label={item}
          onClose={() => deleteDomain(item)}
        />
      ))
    ) : (
      <></>
    );

  const deleteDomain = (value) => {
    const domains = cspDomains.filter((item) => item !== value);

    setCSPSettings({ domains });
  };

  const addDomain = () => {
    const domainsSetting = [...cspDomains];
    const trimmedDomain = domain.trim();
    const domains = trimmedDomain.split(" ");

    domains.map((domain) => {
      if (domain === "" || domainsSetting.includes(domain)) return;

      domainsSetting.push(domain);
    });

    setCSPSettings({ domains: domainsSetting });
    changeDomain("");
  };

  const onChangeDomain = (e) => {
    changeDomain(e.target.value);
  };

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
        <TextInput
          onChange={onChangeDomain}
          value={domain}
          placeholder={t("CSPInputPlaceholder")}
          tabIndex={1}
        />
        <SelectorAddButton onClick={addDomain} />
      </Container>
      <ChipsContainer>{getChips(cspDomains)}</ChipsContainer>
    </>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { cspDomains, getCSPSettings, setCSPSettings } = settingsStore;
  return { cspDomains, getCSPSettings, setCSPSettings };
})(observer(CSP));
