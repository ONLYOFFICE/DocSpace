import React from "react";
import styled from "styled-components";

import Text from "@docspace/components/text";

import { PluginStatus } from "SRC_DIR/helpers/plugins/constants";

const StyledInfo = styled.div`
  display: grid;
  grid-template-columns: 400px 1fr;
  grid-template-rows: 1fr;

  gap: 24px;

  max-width: 684px;

  margin-bottom: 24px;

  .plugin-info-image {
    width: 100%;
  }

  .plugin-info-container {
    display: grid;
    grid-template-columns: max-content 1fr;
    grid-template-rows: auto;

    gap: 16px;

    .row-name {
      color: #858585;
    }
  }
`;

const PluginInfo = ({
  image,
  version,
  author,
  uploader,
  status,
  description,
}) => {
  const getPluginStatusDesc = () => {
    switch (status) {
      case PluginStatus.active:
        return "Not need enter settings";
      case PluginStatus.hide:
      case PluginStatus.pending:
        return "Need enter settings";
    }
  };

  const pluginStatusDesc = getPluginStatusDesc();

  return (
    <StyledInfo>
      <img className="plugin-info-image" src={`/static/plugins/${image}`} />
      <div className="plugin-info-container">
        <Text className="row-name">Version</Text>
        <Text>{version}</Text>
        <Text className="row-name">Author</Text>
        <Text>{author}</Text>
        <Text className="row-name">Uploader </Text>
        <Text>{uploader}</Text>
        <Text className="row-name">Date </Text>
        <Text>{new Date().toJSON().slice(0, 10).replace(/-/g, "/")}</Text>
        <Text className="row-name">Status </Text>
        <Text>{pluginStatusDesc}</Text>
        <Text className="row-name">Description </Text>
        <Text>{description}</Text>
      </div>
    </StyledInfo>
  );
};

export default PluginInfo;
