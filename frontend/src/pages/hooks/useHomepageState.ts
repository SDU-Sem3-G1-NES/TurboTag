import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { LessonClient, LessonDto, LessonFilter } from '../../api/apiClient'

export const useHomePageState = () => {
  const [ownerLessons, setOwnerLessons] = useState<LessonDto[]>([])
  const [starredLessons, setStarredLessons] = useState<LessonDto[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [search, setSearch] = useState<string>('')

  const ownerId = useMemo(() => parseInt(localStorage.getItem('userId') ?? '0', 10), [])

  const [filterValues, setFilterValues] = useState<Partial<LessonFilter>>({
    ownerId,
    userId: ownerId,
    searchText: ''
  })

  const filterValuesJson = useMemo(() => JSON.stringify(filterValues), [filterValues])

  const filter = useMemo(() => {
    return new LessonFilter(JSON.parse(filterValuesJson))
  }, [filterValuesJson])

  const starredFilter = useMemo(() => {
    const base = JSON.parse(filterValuesJson)
    return new LessonFilter({
      ...base,
      isStarred: true,
      userId: ownerId
    })
  }, [filterValuesJson, ownerId])

  const lessonClient = useMemo(() => new LessonClient(), [])

  const loadLessons = useCallback(async () => {
    setLoading(true)
    console.log('Loading lessons with filters:', filter, starredFilter)

    try {
      const [all] = await Promise.all([lessonClient.getAllLessons(filter)])
      const [starred] = await Promise.all([lessonClient.getAllLessons(starredFilter)])

      const allList = Array.isArray(all) ? all : (all?.items ?? [])
      const starredList = Array.isArray(starred) ? starred : (starred?.items ?? [])

      const maxDisplay = 4

      setOwnerLessons(allList.slice(0, maxDisplay))
      setStarredLessons(starredList.slice(0, maxDisplay))
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    } finally {
      setLoading(false)
    }
  }, [lessonClient, filter, starredFilter])

  useEffect(() => {
    loadLessons().then((r) => r)
  }, [loadLessons])

  const handleSearch = useCallback((text: string) => {
    setFilterValues((prev) => {
      if (prev.searchText === text) return prev
      return { ...prev, searchText: text }
    })
  }, [])

  const isFirstRender = useRef(true)

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false
      return
    }

    const delay = setTimeout(() => {
      handleSearch(search)
    }, 500)

    return () => clearTimeout(delay)
  }, [search, handleSearch])

  return {
    ownerLessons,
    starredLessons,
    loading,
    search,
    setSearch,
    handleSearch,
    reload: loadLessons
  }
}
