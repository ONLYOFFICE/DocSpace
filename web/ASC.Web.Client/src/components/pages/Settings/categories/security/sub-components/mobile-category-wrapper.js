import React from "react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import { Base } from "@appserver/components/themes";
import {
  StyledMobileCategoryWrapper,
  StyledArrowRightIcon,
} from "../StyledSecurity";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const MobileCategoryWrapper = (props) => {
  const { title, url, subtitle, onClickLink } = props;

  return (
    <StyledMobileCategoryWrapper>
      <div className="category-item-heading">
        <Link
          className="inherit-title-link header"
          onClick={onClickLink}
          truncate={true}
          href={combineUrl(AppServerConfig.proxyURL, url)}
        >
          {title}
        </Link>
        <StyledArrowRightIcon size="small" />
      </div>
      <Text className="category-item-description">{subtitle}</Text>
    </StyledMobileCategoryWrapper>
  );
};

MobileCategoryWrapper.defaultProps = {
  theme: Base,
};

export default MobileCategoryWrapper;
