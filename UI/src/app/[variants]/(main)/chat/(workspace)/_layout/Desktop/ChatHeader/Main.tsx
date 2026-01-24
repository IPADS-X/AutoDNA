'use client';

import { ActionIcon, Avatar } from '@lobehub/ui';
import { Skeleton } from 'antd';
import { createStyles } from 'antd-style';
import { PanelLeftClose, PanelLeftOpen } from 'lucide-react';
import { parseAsBoolean, useQueryState } from 'nuqs';
import { Suspense, memo } from 'react';
import { useTranslation } from 'react-i18next';
import { Flexbox } from 'react-layout-kit';

import { DESKTOP_HEADER_ICON_SIZE } from '@/const/layoutTokens';
import { useInitAgentConfig } from '@/hooks/useInitAgentConfig';
import { useOpenChatSettings } from '@/hooks/useInterceptingRoutes';
import { useGlobalStore } from '@/store/global';
import { systemStatusSelectors } from '@/store/global/selectors';
import { useSessionStore } from '@/store/session';
import { sessionMetaSelectors, sessionSelectors } from '@/store/session/selectors';

import { start_synthesis_all, change_running, start_sequence, start_sequence_all } from '@/custom/routing';

import Tags from './Tags';
import { Flex } from 'antd/lib';

const useStyles = createStyles(({ css }) => ({
  container: css`
    position: relative;
    overflow: hidden;
    flex: 1;
    max-width: 100%;
  `,
  tag: css`
    flex: none;
    align-items: baseline;
  `,
  title: css`
    overflow: hidden;

    font-size: 14px;
    font-weight: bold;
    line-height: 1;
    text-overflow: ellipsis;
    white-space: nowrap;
  `,
}));

const Main = memo(() => {
  const { t } = useTranslation('chat');
  const { styles } = useStyles();
  useInitAgentConfig();
  const [isPinned] = useQueryState('pinned', parseAsBoolean);

  const [init, isInbox, title, avatar, backgroundColor] = useSessionStore((s) => [
    sessionSelectors.isSomeSessionActive(s),
    sessionSelectors.isInboxSession(s),
    sessionMetaSelectors.currentAgentTitle(s),
    sessionMetaSelectors.currentAgentAvatar(s),
    sessionMetaSelectors.currentAgentBackgroundColor(s),
  ]);

  const openChatSettings = useOpenChatSettings();

  const displayTitle = isInbox ? t('inbox.title') : title;
  const showSessionPanel = useGlobalStore(systemStatusSelectors.showSessionPanel);
  const updateSystemStatus = useGlobalStore((s) => s.updateSystemStatus);

  if (!init)
    return (
      <Flexbox align={'center'} gap={8} horizontal>
        <Flexbox horizontal
          style={{
            display: 'flex',
            justifyContent: 'center', // 水平方向居中
            alignItems: 'center',     // 垂直方向居中
            gap: '8px',
            fontSize: '20px',
            fontWeight: 'bold',
            height: '100%',
          }}
          // onClick={() => { change_running(true); start_sequence_all(); }}
        >
          <div
            onClick={() => { change_running(true); start_synthesis_all("我要写入“饮水思源”"); }}
            // onClick={() => { change_running(true); start_sequence_all(); }}
          ></div>
          {/* {!isPinned && (
          <ActionIcon
            aria-label={t('agents')}
            icon={showSessionPanel ? PanelLeftClose : PanelLeftOpen}
            onClick={() => {
              updateSystemStatus({
                sessionsWidth: showSessionPanel ? 0 : 320,
                showSessionPanel: !showSessionPanel,
              });
            }}
            size={DESKTOP_HEADER_ICON_SIZE}
            title={t('agents')}
          />
        )} */}
          {/* <Avatar
        avatar={avatar}
        background={backgroundColor}
        onClick={() => openChatSettings()}
        size={32}
        title={title}
      /> */}
          {/* <Skeleton
          active
          avatar={{ shape: 'circle', size: 28 }}
          paragraph={false}
          title={{ style: { margin: 0, marginTop: 4 }, width: 200 }}
        /> */}
          <Flexbox align={'center'} className={styles.container} gap={8} horizontal>
            <div className={styles.title}>{displayTitle}</div>
            <Tags />
            {/* <Tags /> */}
          </Flexbox>
        </Flexbox>
      </Flexbox>
    );

  return (
    <Flexbox align={'center'} gap={4} horizontal>
      {!isPinned && (
        <ActionIcon
          aria-label={t('agents')}
          icon={showSessionPanel ? PanelLeftClose : PanelLeftOpen}
          onClick={() => {
            updateSystemStatus({
              sessionsWidth: showSessionPanel ? 0 : 320,
              showSessionPanel: !showSessionPanel,
            });
          }}
          size={DESKTOP_HEADER_ICON_SIZE}
          title={t('agents')}
        />
      )}
      <Avatar
        avatar={avatar}
        background={backgroundColor}
        onClick={() => openChatSettings()}
        size={32}
        title={title}
      />
      <Flexbox align={'center'} className={styles.container} gap={8} horizontal>
        <div className={styles.title}>{displayTitle}</div>
        <Tags />
      </Flexbox>
    </Flexbox>
  );
});

export default () => (
  <Suspense
    fallback={
      <Skeleton
        active
        avatar={{ shape: 'circle', size: 'default' }}
        paragraph={false}
        title={{ style: { margin: 0, marginTop: 8 }, width: 200 }}
      />
    }
  >
    <Main />
  </Suspense>
);
