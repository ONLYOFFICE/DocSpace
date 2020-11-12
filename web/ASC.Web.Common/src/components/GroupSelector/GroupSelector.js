import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "../AdvancedSelector";
import { getGroupList } from "../../api/groups";
import { changeLanguage } from "../../utils";

class GroupSelector extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen } = props;
    this.state = this.getDefaultState(isOpen, []);
  }

  componentDidMount() {
    changeLanguage(i18n);

    getGroupList(this.props.useFake)
      .then((groups) => this.setState({ groups: this.convertGroups(groups) }))
      .catch((error) => console.log(error));
  }

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen)
      this.setState({ isOpen: this.props.isOpen });
  }

  convertGroups = (groups) => {
    return groups
      ? groups.map((g) => {
          return {
            key: g.id,
            label: g.name,
            total: 0,
          };
        })
      : [];
  };

  loadNextPage = ({ startIndex, searchValue }) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}")`
    );

    this.setState({ isNextPageLoading: true }, () => {
      getGroupList(this.props.useFake)
        .then((groups) => {
          const newOptions = this.convertGroups(groups);

          this.setState({
            hasNextPage: false,
            isNextPageLoading: false,
            options: newOptions,
          });
        })
        .catch((error) => console.log(error));
    });
  };

  getDefaultState = (isOpen, groups) => {
    return {
      isOpen: isOpen,
      groups,
      hasNextPage: true,
      isNextPageLoading: false,
    };
  };

  onSearchChanged = (value) => {
    //action("onSearchChanged")(value);
    console.log("Search group", value);
    this.setState({ options: [], hasNextPage: true });
  };

  render() {
    const {
      isOpen,
      groups,
      selectedOptions,
      hasNextPage,
      isNextPageLoading,
    } = this.state;

    const {
      id,
      className,
      style,
      isMultiSelect,
      isDisabled,
      onSelect,
      onCancel,
      t,
      searchPlaceHolderLabel,
      displayType,
      withoutAside,
      embeddedComponent,
      showCounter,
    } = this.props;

    return (
      <AdvancedSelector
        id={id}
        className={className}
        style={style}
        options={groups}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        size={"compact"}
        displayType={displayType}
        selectedOptions={selectedOptions}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={
          searchPlaceHolderLabel || t("SearchPlaceholder")
        }
        selectButtonLabel={t("AddDepartmentsButtonLabel")}
        selectAllLabel={t("SelectAllLabel")}
        emptySearchOptionsLabel={t("EmptySearchOptionsLabel")}
        emptyOptionsLabel={t("EmptyOptionsLabel")}
        loadingLabel={t("LoadingLabel")}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onCancel={onCancel}
        withoutAside={withoutAside}
        embeddedComponent={embeddedComponent}
        showCounter={showCounter}
      />
    );
  }
}

GroupSelector.propTypes = {
  className: PropTypes.oneOf([PropTypes.string, PropTypes.array]),
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  isOpen: PropTypes.bool,
  language: PropTypes.string,
  onCancel: PropTypes.func,
  onSelect: PropTypes.func,
  searchPlaceHolderLabel: PropTypes.string,
  style: PropTypes.object,
  t: PropTypes.func,
  useFake: PropTypes.bool,
  displayType: PropTypes.oneOf(["auto", "aside", "dropdown"]),
  withoutAside: PropTypes.bool,
  embeddedComponent: PropTypes.any,
};

GroupSelector.defaultProps = {
  useFake: false,
  displayType: "auto",
  withoutAside: false,
};

const ExtendedGroupSelector = withTranslation()(GroupSelector);

const GroupSelectorWithI18n = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ExtendedGroupSelector i18n={i18n} {...props} />;
};

export default GroupSelectorWithI18n;
