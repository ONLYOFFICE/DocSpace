import React, { Suspense, lazy } from "react";
import { connect } from "react-redux";
import { Route, Switch } from "react-router-dom";
import { Loader, PageLayout } from "asc-web-components";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent
} from "./Article";
import { SectionHeaderContent } from './Section'

const CommonSettings = lazy(() => import("./sub-components/common"));


const Settings = ({ match, language }) => {

  i18n.changeLanguage(language);

  //console.log("Settings render");
  return (
    <I18nextProvider i18n={i18n}>
      <Suspense
        fallback={<Loader className="pageLoader" type="rombs" size={40} />}
      >
        <PageLayout
          withBodyScroll={false}
          articleHeaderContent={<ArticleHeaderContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={
            <Switch>
              <Route
                // TODO: remove path after added next route
                // path={`${match.path}/common`}
                component={CommonSettings}
              />
            </Switch>
          }
        />


      </Suspense>
    </I18nextProvider >
  );
};

function mapStateToProps(state) {
  return {
    language: state.auth.user.cultureName || state.auth.settings.culture,
  };
}

export default connect(mapStateToProps)(Settings);
