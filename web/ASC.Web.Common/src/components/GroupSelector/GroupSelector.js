import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "../AdvancedSelector";
import { getGroupList } from "../../api/groups";

class GroupSelector extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen } = props;
    this.state = this.getDefaultState(isOpen, []);
  }

  componentDidMount() {
    const { language } = this.props;
    i18n.changeLanguage(language);

    getGroupList(this.props.useFake)
      .then(groups => this.setState({ groups: this.convertGroups(groups) }))
      .catch(error => console.log(error));
  }

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen)
      this.setState({ isOpen: this.props.isOpen });
  }

  convertGroups = groups => {
    return groups
      ? groups.map(g => {
          return {
            key: g.id,
            label: g.name,
            total: 0
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
        .then(groups => {
          const newOptions = this.convertGroups(groups);

          this.setState({
            hasNextPage: false,
            isNextPageLoading: false,
            options: newOptions
          });
        })
        .catch(error => console.log(error));
    });
  };

  getDefaultState = (isOpen, groups) => {
    return {
      isOpen: isOpen,
      groups,
      hasNextPage: true,
      isNextPageLoading: false
    };
  };

  onSearchChanged = value => {
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
      isNextPageLoading
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
      searchPlaceHolderLabel
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
        displayType={"auto"}
        selectedOptions={selectedOptions}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={searchPlaceHolderLabel || t("SearchPlaceholder")}
        selectButtonLabel={t("AddDepartmentsButtonLabel")}
        selectAllLabel={t("SelectAllLabel")}
        emptySearchOptionsLabel={t("EmptySearchOptionsLabel")}
        emptyOptionsLabel={t("EmptyOptionsLabel")}
        loadingLabel={t("LoadingLabel")}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onCancel={onCancel}
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
};

GroupSelector.defaultProps = {
  useFake: false
};

const ExtendedGroupSelector = withTranslation()(GroupSelector);

const GroupSelectorWithI18n = props => {
  const { language } = props;
  i18n.changeLanguage(language);

  return <ExtendedGroupSelector i18n={i18n} {...props} />;
};

GroupSelectorWithI18n.propTypes = {
  language: PropTypes.string
};

function mapStateToProps(state) {
  return {
    language:
      state.auth &&
      ((state.auth.user && state.auth.user.cultureName) ||
        (state.auth.settings && state.auth.settings.culture))
  };
}

export default connect(mapStateToProps)(GroupSelectorWithI18n);
