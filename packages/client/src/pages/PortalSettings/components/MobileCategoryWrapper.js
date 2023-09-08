import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import { Base } from "@docspace/components/themes";
import { StyledMobileCategoryWrapper, StyledArrowRightIcon } from "./styles";
import { combineUrl } from "@docspace/common/utils";

const MobileCategoryWrapper = (props) => {
  const { title, url, subtitle, onClickLink } = props;

  return (
    <StyledMobileCategoryWrapper>
      <div className="category-item-heading">
        <Link
          className="inherit-title-link header settings_unavailable"
          onClick={onClickLink}
          truncate={true}
          href={combineUrl(window.DocSpaceConfig?.proxy?.url, url)}
        >
          {title}
        </Link>
        <StyledArrowRightIcon className="settings_unavailable" size="small" />
      </div>
      <Text className="category-item-description settings_unavailable">
        {subtitle}
      </Text>
    </StyledMobileCategoryWrapper>
  );
};

MobileCategoryWrapper.defaultProps = {
  theme: Base,
};

export default MobileCategoryWrapper;
