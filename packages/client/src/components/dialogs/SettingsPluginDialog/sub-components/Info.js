import React from "react";
import styled, { css } from "styled-components";

import { getCookie } from "@docspace/common/utils";
import { LANGUAGE } from "@docspace/common/constants";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import getCorrectDate from "@docspace/components/utils/getCorrectDate";

import { PluginStatus } from "SRC_DIR/helpers/plugins/constants";
import { Base } from "@docspace/components/themes";

const StyledContainer = styled.div`
  width: 100%;

  ${(props) =>
    props.withDelete &&
    css`
      margin-bottom: 24px;
    `}
`;

const StyledSeparator = styled.div`
  width: 100%;
  height: 1px;

  margin: 24px 0;

  background-color: ${(props) => props.theme.plugins.borderColor};
`;

StyledSeparator.defaultProps = { theme: Base };

const StyledInfo = styled.div`
  margin-top: 24px;

  width: 100%;
  height: auto;

  display: grid;

  grid-template-columns: max-content 1fr;

  gap: 8px 24px;
`;

const Info = ({ t, plugin, withDelete }) => {
  const locale = getCookie(LANGUAGE) || "en";
  const uploadDate = plugin.createOn && getCorrectDate(locale, plugin.createOn);

  const pluginStatus =
    plugin.status === PluginStatus.active
      ? t("NotNeedSettings")
      : t("NeedSettings");

  return (
    <StyledContainer withDelete={withDelete}>
      <StyledSeparator />
      <Text fontSize={"14px"} fontWeight={600} lineHeight={"16px"} noSelect>
        {t("Metadata")}
      </Text>
      <StyledInfo>
        {plugin.author && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Files:ByAuthor")}
            </Text>
            <Text
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              noSelect
            >
              {plugin.author}
            </Text>
          </>
        )}

        {plugin.version && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Common:Version")}
            </Text>
            <Text
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              noSelect
            >
              {plugin.version}
            </Text>
          </>
        )}

        {!plugin.system && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Common:Uploader")}
            </Text>
            <Text
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              noSelect
            >
              {plugin.createBy}
            </Text>
          </>
        )}

        {!plugin.system && uploadDate && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Common:UploadDate")}
            </Text>
            <Text
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              noSelect
            >
              {uploadDate}
            </Text>
          </>
        )}

        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          {t("People:UserStatus")}
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {pluginStatus}
        </Text>

        {plugin.homePage && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Common:Homepage")}
            </Text>
            <Link
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              type={"page"}
              href={plugin?.homePage}
              target={"_blank"}
              noSelect
              isHovered
            >
              {plugin.homePage}
            </Link>
          </>
        )}
        {plugin.description && (
          <>
            <Text
              fontSize={"13px"}
              fontWeight={400}
              lineHeight={"20px"}
              noSelect
              truncate
            >
              {t("Common:Description")}
            </Text>
            <Text
              fontSize={"13px"}
              fontWeight={600}
              lineHeight={"20px"}
              noSelect
            >
              {plugin.description}
            </Text>
          </>
        )}
      </StyledInfo>
    </StyledContainer>
  );
};

export default Info;
