import { useCallback, useEffect, useMemo, useState } from 'react'
import { LessonClient, LessonDto, LessonFilter } from '../../api/apiClient'

export const useHomePageState = () => {
  const [ownerLessons, setOwnerLessons] = useState<LessonDto[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [search, setSearch] = useState<string>('')

  // Flat filter values so we can easily update properties
  const [filterValues, setFilterValues] = useState<Partial<LessonFilter>>({
    ownerId: parseInt(localStorage.getItem('userId') ?? '0', 10),
    searchText: ''
  })

  // Memoize actual filter object to prevent new reference every render
  const filter = useMemo(() => new LessonFilter({ ...filterValues }), [filterValues])

  const lessonClient = useMemo(() => new LessonClient(), [])

  const loadLessons = useCallback(async () => {
    setLoading(true)
    try {
      const data = await lessonClient.getAllLessons(filter)
      setOwnerLessons(Array.isArray(data) ? data : data.items || [])
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    } finally {
      setLoading(false)
    }
  }, [lessonClient, filter])

  // Initial + on-filter-change load
  useEffect(() => {
    loadLessons()
  }, [loadLessons])

  // Called when user presses Enter or clicks the search icon
  const handleSearch = useCallback((text: string) => {
    setFilterValues((prev) => ({
      ...prev,
      searchText: text
    }))
  }, [])

  // Called when user types into the search bar (debounced)
  useEffect(() => {
    const delay = setTimeout(() => {
      handleSearch(search)
    }, 500)

    return () => clearTimeout(delay)
  }, [search, handleSearch])

  return {
    ownerLessons,
    loading,
    search,
    setSearch,
    handleSearch
  }
}
